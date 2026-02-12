using System.Collections;
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
        
        private Coroutine _progressCoroutine;

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
            // 이전 진행 코루틴이 있다면 중지 (레이스 컨디션 방지 핵심)
            if (_progressCoroutine != null)
            {
                StopCoroutine(_progressCoroutine);
            }
            _progressCoroutine = StartCoroutine(ProcessDialogueProgressAsync(evt));
        }

        private IEnumerator ProcessDialogueProgressAsync(DialogueProgressEvent evt)
        {
            if (_currentDisplayedImageInstance != null && _currentDisplayedImageInstance.activeSelf)
            {
                _currentDisplayedImageInstance.SetActive(false);
            }

            var node = evt.NextDialogueNode;
            CharacterInformationSO characterInfo = null;

            // 1. 캐릭터 정보 로드 (CharacterInformationSO)
            if (node.CharacterInformSO != null && node.CharacterInformSO.RuntimeKeyIsValid())
            {
                // 이미 로드된 에셋인지 확인하여 중복 로드 에러 방지
                if (node.CharacterInformSO.OperationHandle.IsValid())
                {
                    var handle = node.CharacterInformSO.OperationHandle.Convert<CharacterInformationSO>();
                    if (!handle.IsDone) yield return handle;
                    characterInfo = handle.Result;
                }
                else
                {
                    ReleaseCurrentCharacter();
                    _currentCharacterHandle = node.CharacterInformSO.LoadAssetAsync();
                    yield return _currentCharacterHandle;

                    if (_currentCharacterHandle.IsValid() && _currentCharacterHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        characterInfo = _currentCharacterHandle.Result;
                    }
                }
            }
            
            // 2. 캐릭터 이름 및 스프라이트 처리
            if (characterInfo != null && !string.IsNullOrEmpty(characterInfo.CharacterName))
            {
                nameTag.SetActive(true);
                nameText.text = characterInfo.CharacterName;
                
                if (characterInfo.CharacterEmotions != null && 
                    characterInfo.CharacterEmotions.TryGetValue(node.CharacterEmotion, out var spriteRef) && 
                    spriteRef != null && 
                    spriteRef.RuntimeKeyIsValid())
                {
                    // 스프라이트 중복 로드 체크
                    if (spriteRef.OperationHandle.IsValid())
                    {
                        var sHandle = spriteRef.OperationHandle.Convert<Sprite>();
                        if (!sHandle.IsDone) yield return sHandle;
                        characterImage.sprite = sHandle.Result;
                        characterImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        ReleaseCurrentCharacterSprite();
                        _currentCharacterSpriteHandle = spriteRef.LoadAssetAsync();
                        yield return _currentCharacterSpriteHandle;

                        if (_currentCharacterSpriteHandle.IsValid() && _currentCharacterSpriteHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            characterImage.sprite = _currentCharacterSpriteHandle.Result;
                            characterImage.gameObject.SetActive(true);
                        }
                        else
                        {
                            characterImage.gameObject.SetActive(false);
                        }
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
            
            // 3. UI 연출 및 텍스트 출력
            backgroundImage.sprite = evt.BackgroundImage;
            
            // 위치 설정
            RectTransform targetPos = (node.NameTagPosition == NameTagPositionType.Left) ? nameTagLeft : nameTagRight;
            nameTag.transform.position = targetPos.position;
            characterImage.transform.position = targetPos.position;
            
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _fullDialogueText = node.DialogueDetail;
            _typingCoroutine = StartCoroutine(TypeDialogue(_fullDialogueText));
            
            _progressCoroutine = null;
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