using System.Collections;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Member.LS.Code.Dialogue;
using UnityEngine;

namespace Code.MainSystem.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private enum DialogueState { Idle, Active }
        private DialogueState _state = DialogueState.Idle;

        private DialogueInformationSO _dialogueInformationSO;

        private int _dialogueIndex = -1;
        private bool _acceptingInput = false;

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

            ProcessCurrentNode();
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

            ProcessCurrentNode();
        }

        private void OnChoiceSelected(DialogueChoiceSelectedEvent evt)
        {
            if (_state != DialogueState.Active) return;

            if (evt.Events != null)
            {
                foreach (var eventSO in evt.Events)
                {
                    eventSO.RaiseDialogueEvent();
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
                return;
            }

            _acceptingInput = true;
            ProcessCurrentNode();
        }

        private void ProcessCurrentNode()
        {
            var currentNode = _dialogueInformationSO.DialogueDetails[_dialogueIndex];
            
            if (currentNode.Events != null)
            {
                foreach (var eventSO in currentNode.Events)
                {
                    eventSO.RaiseDialogueEvent();
                }
            }
            
            int bgIndex = currentNode.BackgroundIndex;
            if (bgIndex < 0 || bgIndex >= _dialogueInformationSO.DialogueBackground.Count)
            {
                Debug.LogError($"Invalid background index {bgIndex} in dialogue node {_dialogueIndex}. Using index 0 as fallback.");
                bgIndex = 0;
            }
            
            var eventData = new DialogueProgressEvent(currentNode, _dialogueInformationSO.DialogueBackground[bgIndex]);
            Bus<DialogueProgressEvent>.Raise(eventData);

            if (currentNode.Choices != null && currentNode.Choices.Count > 0)
            {
                _acceptingInput = false;
                Bus<DialogueShowChoiceEvent>.Raise(new DialogueShowChoiceEvent(currentNode.Choices));
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
            Bus<DialogueEndEvent>.Raise(new DialogueEndEvent());
            _dialogueIndex = -1;
            _dialogueInformationSO = null;
            _state = DialogueState.Idle;
            _acceptingInput = false;
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