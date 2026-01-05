using System.Collections;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Member.LS.Code.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        }

        private void OnDestroy()
        {
            characterImage.gameObject.SetActive(false);
            if (_currentDisplayedImageInstance != null)
            {
                Destroy(_currentDisplayedImageInstance);
            }
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
            if (_currentDisplayedImageInstance != null && _currentDisplayedImageInstance.activeSelf)
            {
                _currentDisplayedImageInstance.SetActive(false);
            }

            var node = evt.NextDialogueNode;

            if (node.CharacterInformSO != null && !string.IsNullOrEmpty(node.CharacterInformSO.CharacterName))
            {
                nameTag.SetActive(true);
                nameText.text = node.CharacterInformSO.CharacterName;

                if (node.CharacterInformSO.CharacterEmotions.TryGetValue(node.CharacterEmotion, out var characterSprite) && characterSprite != null)
                {
                    characterImage.sprite = characterSprite;
                    characterImage.gameObject.SetActive(true);
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
            
            // if (dialogueUIParent != null)
            // {
            //     dialogueUIParent.SetActive(false);
            // }
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

        private void OnImageDisplayEvent(ImageDisplayEvent evt)
        {
            if (_currentDisplayedImageInstance != null && _currentDisplayedImageInstance.activeSelf)
            {
                _currentDisplayedImageInstance.SetActive(false);
            }

            if (_currentDisplayedImageInstance == null || _currentDisplayedImageInstance.name != evt.ImagePrefabToDisplay.name)
            {
                if (_currentDisplayedImageInstance != null)
                {
                    Destroy(_currentDisplayedImageInstance);
                }
                _currentDisplayedImageInstance = Instantiate(evt.ImagePrefabToDisplay, imageDisplayParent);
                _currentDisplayedImageInstance.name = evt.ImagePrefabToDisplay.name;
            }
            
            _currentDisplayedImageInstance.SetActive(true);
        }
    }
}