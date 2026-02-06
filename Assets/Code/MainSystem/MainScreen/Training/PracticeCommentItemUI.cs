using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI thoughtsText;
        [SerializeField] private List<StatUpDownComment> statChangeItems;

        [Header("Typing Settings")]
        [SerializeField] private float typingSpeed = 0.03f;
        [SerializeField] private float delayBetweenSections = 0.3f;
        
        [Header("Font Settings")]
        [SerializeField] private bool useHandwritingFont = true;

        private CancellationTokenSource _animationCTS;
        public CommentData _currentData;
        private TMP_FontAsset _handwritingFont;

        private void OnDestroy()
        {
            _animationCTS?.Cancel();
            _animationCTS?.Dispose();
        }

        public void SetHandwritingFont(TMP_FontAsset font)
        {
            _handwritingFont = font;
            
            if (useHandwritingFont && _handwritingFont != null)
            {
                ApplyHandwritingFont();
            }
        }

        private void ApplyHandwritingFont()
        {
            if (_handwritingFont == null) return;
            
            if (titleText != null) titleText.font = _handwritingFont;
            if (contentText != null) contentText.font = _handwritingFont;
            if (thoughtsText != null) thoughtsText.font = _handwritingFont;
        }

        public void Setup(CommentData data)
        {
            _currentData = data;
            
            _animationCTS?.Cancel();
            _animationCTS?.Dispose();
            _animationCTS = new CancellationTokenSource();
            
            if (useHandwritingFont && _handwritingFont != null)
            {
                ApplyHandwritingFont();
            }
            
            titleText.text = "";
            contentText.text = "";
            thoughtsText.text = "";
            
            foreach (var item in statChangeItems)
            {
                item.gameObject.SetActive(false);
                
                var cg = item.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = item.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
            }
            
            PlayTypingAnimation(data, _animationCTS.Token).Forget();
        }

        private async UniTaskVoid PlayTypingAnimation(CommentData data, CancellationToken token)
        {
            try
            {
                await TypeText(titleText, data.title, token);
                await UniTask.Delay((int)(delayBetweenSections * 1000), cancellationToken: token);
                
                await TypeText(contentText, data.content, token);
                await UniTask.Delay((int)(delayBetweenSections * 1000), cancellationToken: token);
                
                if (!string.IsNullOrEmpty(data.thoughts))
                {
                    await TypeText(thoughtsText, data.thoughts, token);
                    await UniTask.Delay((int)(delayBetweenSections * 1000), cancellationToken: token);
                }
                
                await ShowStatChanges(data.statChanges, token);
            }
            catch (System.OperationCanceledException)
            {
                SkipToComplete(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PracticeCommentItemUI] Error in typing animation: {e.Message}");
            }
        }

        private async UniTask TypeText(TextMeshProUGUI textComponent, string fullText, CancellationToken token)
        {
            if (string.IsNullOrEmpty(fullText)) return;

            textComponent.text = "";
            
            for (int i = 0; i <= fullText.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                textComponent.text = fullText.Substring(0, i);
                await UniTask.Delay((int)(typingSpeed * 1000), cancellationToken: token);
            }
        }

        private async UniTask ShowStatChanges(List<StatChangeInfo> statChanges, CancellationToken token)
        {
            for (int i = 0; i < statChangeItems.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                
                if (i < statChanges.Count)
                {
                    var item = statChangeItems[i];
                    item.gameObject.SetActive(true);
                    item.Setup(statChanges[i]);
                    
                    var canvasGroup = item.GetComponent<CanvasGroup>();
                    var rectTransform = item.GetComponent<RectTransform>();
                    
                    if (canvasGroup != null && rectTransform != null)
                    {
                        canvasGroup.alpha = 0f;
                        Vector3 originalScale = rectTransform.localScale;
                        rectTransform.localScale = Vector3.zero;
                        
                        var fadeTask = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 0.3f)
                            .SetEase(Ease.OutCubic)
                            .AsyncWaitForCompletion()
                            .AsUniTask();

                        var scaleTask = rectTransform.DOScale(originalScale, 0.3f)
                            .SetEase(Ease.OutBack)
                            .AsyncWaitForCompletion()
                            .AsUniTask();

                        await UniTask.WhenAll(fadeTask, scaleTask);
                    }

                    await UniTask.Delay(150, cancellationToken: token); 
                }
                else
                {
                    statChangeItems[i].gameObject.SetActive(false);
                }
            }
        }

        public void SkipToComplete(CommentData data)
        {
            _animationCTS?.Cancel();
            
            
            titleText.text = data.title;
            contentText.text = data.content;
            thoughtsText.text = string.IsNullOrEmpty(data.thoughts) ? "" : data.thoughts;

            for (int i = 0; i < statChangeItems.Count; i++)
            {
                if (i < data.statChanges.Count)
                {
                    statChangeItems[i].gameObject.SetActive(true);
                    statChangeItems[i].Setup(data.statChanges[i]);
                    
                    var cg = statChangeItems[i].GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = 1f;
                    
                    var rt = statChangeItems[i].GetComponent<RectTransform>();
                    if (rt != null) rt.localScale = Vector3.one;
                }
                else
                {
                    statChangeItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}