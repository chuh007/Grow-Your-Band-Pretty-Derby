using System.Collections;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Member.LS.Code.Dialogue;
using Member.LS.Code.Dialogue.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.MainSystem.Dialogue.UI
{
    public class DialogueUIManager : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject dialogueUIParent;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private GameObject nameTag;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Transform imageDisplayParent;

        [Header("Name Tag Positions")]
        [SerializeField] private RectTransform nameTagLeft;
        [SerializeField] private RectTransform nameTagRight;
        
        [Header("Typing Effect")]
        [SerializeField] private float typingSpeed = 0.05f;

        private Coroutine _typingCoroutine;
        private bool _isTyping;
        private string _fullDialogueText;
        
        private GameObject _currentDisplayedImageInstance;
        private AsyncOperationHandle<CharacterInformationSO> _currentCharacterHandle;
        private AsyncOperationHandle<Sprite> _currentCharacterSpriteHandle;

        private void Awake()
        {
            if (dialogueUIParent != null)
            {
                dialogueUIParent.SetActive(false);
            }
        }

        private void OnEnable()
        {
            Bus<DialogueStartEvent>.OnEvent += OnDialogueStart;
            Bus<DialogueProgressEvent>.OnEvent += OnDialogueProgress;
            Bus<DialogueEndEvent>.OnEvent += OnDialogueEnd;
            Bus<UIContinueButtonPressedEvent>.OnEvent += OnUIContinueButtonPressed;
            Bus<ImageDisplayEvent>.OnEvent += OnImageDisplayEvent;
        }

        private void OnDisable()
        {
            Bus<DialogueStartEvent>.OnEvent -= OnDialogueStart;
            Bus<DialogueProgressEvent>.OnEvent -= OnDialogueProgress;
            Bus<DialogueEndEvent>.OnEvent -= OnDialogueEnd;
            Bus<UIContinueButtonPressedEvent>.OnEvent -= OnUIContinueButtonPressed;
            Bus<ImageDisplayEvent>.OnEvent -= OnImageDisplayEvent;
    
            ReleaseCurrentCharacter();
            ReleaseCurrentCharacterSprite();
            ReleaseCurrentImage();
        }

        private void OnDestroy()
        {
            characterImage.gameObject.SetActive(false);
    
            ReleaseCurrentCharacter();
            ReleaseCurrentCharacterSprite();
            ReleaseCurrentImage();
        }

        private void OnDialogueStart(DialogueStartEvent obj)
        {
            if (dialogueUIParent != null)
            {
                dialogueUIParent.SetActive(true);
            }
        }

        private void OnDialogueProgress(DialogueProgressEvent evt)
        {
            StartCoroutine(ProcessDialogueProgressAsync(evt));
        }

        private IEnumerator ProcessDialogueProgressAsync(DialogueProgressEvent evt)
        {
            if (_currentDisplayedImageInstance != null && _currentDisplayedImageInstance.activeSelf)
            {
                _currentDisplayedImageInstance.SetActive(false);
            }

            var node = evt.NextDialogueNode;
            
            CharacterInformationSO characterInfo = null;
            if (node.CharacterInformSO != null && node.CharacterInformSO.RuntimeKeyIsValid())
            {
                ReleaseCurrentCharacter();
                
                _currentCharacterHandle = node.CharacterInformSO.LoadAssetAsync();
                yield return _currentCharacterHandle;

                if (_currentCharacterHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    characterInfo = _currentCharacterHandle.Result;
                }
                else
                {
                    Debug.LogError("Failed to load character information");
                }
            }
            
            if (characterInfo != null && !string.IsNullOrEmpty(characterInfo.CharacterName))
            {
                nameTag.SetActive(true);
                nameText.text = characterInfo.CharacterName;
                
                if (characterInfo.CharacterEmotions != null && 
                    characterInfo.CharacterEmotions.TryGetValue(node.CharacterEmotion, out var spriteRef) && 
                    spriteRef != null && 
                    spriteRef.RuntimeKeyIsValid())
                {
                    ReleaseCurrentCharacterSprite();
                    
                    _currentCharacterSpriteHandle = spriteRef.LoadAssetAsync();
                    yield return _currentCharacterSpriteHandle;

                    if (_currentCharacterSpriteHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        characterImage.sprite = _currentCharacterSpriteHandle.Result;
                        characterImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("Failed to load character emotion sprite");
                        characterImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    characterImage.gameObject.SetActive(false);
                }
            }
            else
            {
                nameTag.SetActive(false);
                characterImage.gameObject.SetActive(false);
            }
            
            backgroundImage.sprite = evt.BackgroundImage;
            
            if (node.NameTagPosition == NameTagPositionType.Left)
            {
                nameTag.transform.position = nameTagLeft.position;
                characterImage.transform.position = nameTagLeft.position;
            }
            else
            {
                nameTag.transform.position = nameTagRight.position;
                characterImage.transform.position = nameTagRight.position;
            }
            
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _fullDialogueText = node.DialogueDetail;
            _typingCoroutine = StartCoroutine(TypeDialogue(_fullDialogueText));
        }
        
        private void OnDialogueEnd(DialogueEndEvent e)
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }
            _isTyping = false;

            if (_currentDisplayedImageInstance != null && _currentDisplayedImageInstance.activeSelf)
            {
                _currentDisplayedImageInstance.SetActive(false);
            }
            
            ReleaseCurrentCharacter();
            ReleaseCurrentCharacterSprite();
        }
        
        private void OnUIContinueButtonPressed(UIContinueButtonPressedEvent e)
        {
            if (_isTyping)
            {
                if (_typingCoroutine != null)
                {
                    StopCoroutine(_typingCoroutine);
                    _typingCoroutine = null;
                }
                dialogueText.text = _fullDialogueText;
                _isTyping = false;
            }
            else
            {
                Bus<ContinueDialogueEvent>.Raise(new ContinueDialogueEvent());
            }
        }

        private IEnumerator TypeDialogue(string dialogue)
        {
            _isTyping = true;
            dialogueText.text = "";
            foreach (char letter in dialogue.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            _typingCoroutine = null;
            _isTyping = false;
        }

        private AsyncOperationHandle<GameObject> _currentImageHandle;

        private void OnImageDisplayEvent(ImageDisplayEvent evt)
        {
            StartCoroutine(LoadAndDisplayImageAsync(evt.ImagePrefabReference));
        }

        private IEnumerator LoadAndDisplayImageAsync(AssetReferenceGameObject prefabRef)
        {
            if (_currentDisplayedImageInstance != null)
            {
                if (_currentDisplayedImageInstance.activeSelf)
                {
                    _currentDisplayedImageInstance.SetActive(false);
                }
                
                if (_currentImageHandle.IsValid())
                {
                    Addressables.ReleaseInstance(_currentImageHandle);
                }
        
                _currentDisplayedImageInstance = null;
            }

            if (prefabRef != null && prefabRef.RuntimeKeyIsValid())
            {
                _currentImageHandle = prefabRef.InstantiateAsync(imageDisplayParent);
                yield return _currentImageHandle;

                if (_currentImageHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _currentDisplayedImageInstance = _currentImageHandle.Result;
                    _currentDisplayedImageInstance.SetActive(true);
                }
                else
                {
                    Debug.LogError("Failed to load image prefab");
                }
            }
        }
        
        private void ReleaseCurrentImage()
        {
            if (_currentImageHandle.IsValid())
            {
                Addressables.ReleaseInstance(_currentImageHandle);
            }
        }

        private void ReleaseCurrentCharacter()
        {
            if (_currentCharacterHandle.IsValid())
            {
                Addressables.Release(_currentCharacterHandle);
            }
        }

        private void ReleaseCurrentCharacterSprite()
        {
            if (_currentCharacterSpriteHandle.IsValid())
            {
                Addressables.Release(_currentCharacterSpriteHandle);
            }
        }
    }
}