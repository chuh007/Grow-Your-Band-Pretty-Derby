using System.Collections;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.Dialogue.DialogueEvent;
using Member.LS.Code.Dialogue;
using Member.LS.Code.Dialogue.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.MainSystem.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private enum DialogueState { Idle, Active }
        private DialogueState _state = DialogueState.Idle;

        private DialogueInformationSO _dialogueInformationSO;

        private int _dialogueIndex = -1;
        private bool _acceptingInput = false;
        private AsyncOperationHandle<Sprite> _currentSpriteHandle;
        private List<AsyncOperationHandle> _loadedHandles = new List<AsyncOperationHandle>();

        private void OnEnable()
        {
            Bus<ContinueDialogueEvent>.OnEvent += OnContinueDialogue;
            Bus<DialogueSkipEvent>.OnEvent += OnDialogueSkip;
            Bus<DialogueStartEvent>.OnEvent += OnDialogueStart;
            Bus<DialogueChoiceSelectedEvent>.OnEvent += OnChoiceSelected;
        }

        private void OnDisable()
        {
            Bus<ContinueDialogueEvent>.OnEvent -= OnContinueDialogue;
            Bus<DialogueSkipEvent>.OnEvent -= OnDialogueSkip;
            Bus<DialogueStartEvent>.OnEvent -= OnDialogueStart;
            Bus<DialogueChoiceSelectedEvent>.OnEvent -= OnChoiceSelected;
            
            ReleaseCurrentSprite();
            ReleaseAllHandles();
        }

        private void OnDialogueStart(DialogueStartEvent evt)
        {
            if (evt.DialogueSO.DialogueBackground == null || evt.DialogueSO.DialogueBackground.Count == 0)
            {
                Debug.LogError("Cannot start dialogue: DialogueInformationSO has no backgrounds.");
                return;
            }
            
            StopAllCoroutines();
            StartCoroutine(StartDialogueProcess(evt));
        }

        private IEnumerator StartDialogueProcess(DialogueStartEvent evt)
        {
            yield return null;

            _dialogueInformationSO = evt.DialogueSO;
            _dialogueIndex = 0;
            _state = DialogueState.Active;
            _acceptingInput = false;
            StartCoroutine(EnableInputAfterFrame());

            yield return StartCoroutine(ProcessCurrentNodeAsync());
        }

        private void OnContinueDialogue(ContinueDialogueEvent e)
        { 
            if (_state != DialogueState.Active || !_acceptingInput) return;
            
            if (_dialogueInformationSO.DialogueBackground == null || _dialogueInformationSO.DialogueBackground.Count == 0)
            {
                Debug.LogError("Cannot continue dialogue: DialogueInformationSO has no backgrounds.");
                EndDialogue();
                return;
            }
            
            _dialogueIndex++;

            if (_dialogueIndex >= _dialogueInformationSO.DialogueDetails.Count)
            {
                EndDialogue();
                return;
            }

            StartCoroutine(ProcessCurrentNodeAsync());
        }

        private void OnChoiceSelected(DialogueChoiceSelectedEvent evt)
        {
            if (_state != DialogueState.Active) return;

            StartCoroutine(ProcessChoiceEventsAsync(evt));
        }

        private IEnumerator ProcessChoiceEventsAsync(DialogueChoiceSelectedEvent evt)
        {
            if (evt.Events != null && evt.Events.Count > 0)
            {
                foreach (var eventRef in evt.Events)
                {
                    var eventHandle = eventRef.LoadAssetAsync();
                    _loadedHandles.Add(eventHandle);
                    yield return eventHandle;

                    if (eventHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        eventHandle.Result.RaiseDialogueEvent();
                    }
                    else
                    {
                        Debug.LogError("Failed to load dialogue event from choice");
                    }
                }
            }

            int nextIndex = evt.NextNodeIndex;
            if (nextIndex < 0)
            {
                nextIndex = _dialogueIndex + 1;
            }

            _dialogueIndex = nextIndex;

            if (_dialogueIndex >= _dialogueInformationSO.DialogueDetails.Count)
            {
                EndDialogue();
                yield break;
            }

            _acceptingInput = true;
            yield return StartCoroutine(ProcessCurrentNodeAsync());
        }

        private IEnumerator ProcessCurrentNodeAsync()
        {
            var currentNode = _dialogueInformationSO.DialogueDetails[_dialogueIndex];
            
            if (currentNode.Events != null && currentNode.Events.Count > 0)
            {
                foreach (var eventRef in currentNode.Events)
                {
                    var eventHandle = eventRef.LoadAssetAsync();
                    _loadedHandles.Add(eventHandle);
                    yield return eventHandle;

                    if (eventHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        eventHandle.Result.RaiseDialogueEvent();
                    }
                    else
                    {
                        Debug.LogError("Failed to load dialogue event from node");
                    }
                }
            }
            
            CharacterInformationSO characterInfo = null;
            if (currentNode.CharacterInformSO != null && currentNode.CharacterInformSO.RuntimeKeyIsValid())
            {
                var charHandle = currentNode.CharacterInformSO.LoadAssetAsync();
                _loadedHandles.Add(charHandle);
                yield return charHandle;

                if (charHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    characterInfo = charHandle.Result;
                }
                else
                {
                    Debug.LogError("Failed to load character information");
                }
            }
            
            int bgIndex = currentNode.BackgroundIndex;
            if (bgIndex < 0 || bgIndex >= _dialogueInformationSO.DialogueBackground.Count)
            {
                Debug.LogError($"Invalid background index {bgIndex} in dialogue node {_dialogueIndex}. Using index 0 as fallback.");
                bgIndex = 0;
            }
            
            ReleaseCurrentSprite();
            
            _currentSpriteHandle = _dialogueInformationSO.DialogueBackground[bgIndex].LoadAssetAsync<Sprite>();
            yield return _currentSpriteHandle;

            if (_currentSpriteHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var eventData = new DialogueProgressEvent(currentNode, _currentSpriteHandle.Result);
                Bus<DialogueProgressEvent>.Raise(eventData);

                if (currentNode.Choices != null && currentNode.Choices.Count > 0)
                {
                    _acceptingInput = false;
                    Bus<DialogueShowChoiceEvent>.Raise(new DialogueShowChoiceEvent(currentNode.Choices));
                }
            }
            else
            {
                Debug.LogError($"Failed to load background sprite at index {bgIndex}");
            }
        }

        private void OnDialogueSkip(DialogueSkipEvent e)
        { 
            if (_state != DialogueState.Active || !_acceptingInput) return;
            
            EndDialogue();
        }

        private void EndDialogue()
        {
            StopAllCoroutines();
            ReleaseCurrentSprite();
            ReleaseAllHandles();
            Bus<DialogueEndEvent>.Raise(new DialogueEndEvent());
            _dialogueIndex = -1;
            _dialogueInformationSO = null;
            _state = DialogueState.Idle;
            _acceptingInput = false;
        }
        
        private void ReleaseCurrentSprite()
        {
            if (_currentSpriteHandle.IsValid())
            {
                Addressables.Release(_currentSpriteHandle);
            }
        }

        private void ReleaseAllHandles()
        {
            foreach (var handle in _loadedHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _loadedHandles.Clear();
        }
        
        private IEnumerator EnableInputAfterFrame()
        { 
            yield return new WaitForEndOfFrame();
            
            if (_dialogueInformationSO != null && _dialogueIndex >= 0 && _dialogueIndex < _dialogueInformationSO.DialogueDetails.Count)
            {
                var node = _dialogueInformationSO.DialogueDetails[_dialogueIndex];
                if (node.Choices != null && node.Choices.Count > 0)
                {
                    yield break;
                }
            }
            
            _acceptingInput = true;
        }
    }
}