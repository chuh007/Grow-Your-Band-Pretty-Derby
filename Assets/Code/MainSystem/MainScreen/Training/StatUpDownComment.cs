using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 코멘트 창에서 스탯 증감을 표시하는 UI 컴포넌트
    /// 
    /// 기능:
    /// - 스탯 아이콘 표시
    /// - 증가(↑)/감소(↓) 화살표 표시
    /// - 증감량 텍스트 표시 (+N / -N)
    /// - 색상 구분 (증가=녹색, 감소=빨간색)
    /// 
    /// 사용처:
    /// - PracticeCommentItemUI의 statChanges 리스트에 포함
    /// - 각 스탯 변화마다 하나씩 생성
    /// </summary>
    public class StatUpDownComment : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Components")]
        [SerializeField] private Image statIconImage;        // 스탯 아이콘 (예: 보컬 아이콘, 컨디션 아이콘)
        [SerializeField] private GameObject goUpImage;       // 증가 화살표 (↑)
        [SerializeField] private GameObject goDownImage;     // 감소 화살표 (↓)
        [SerializeField] private TextMeshProUGUI valueSucsseText;  // 증가 시 텍스트 (오타: Success)
        [SerializeField] private TextMeshProUGUI valueFaillText;   // 감소 시 텍스트 (오타: Fail)

        [Header("Color Settings")]
        [SerializeField] private Color upColor = Color.green;   // 증가 색상
        [SerializeField] private Color downColor = Color.red;   // 감소 색상

        #endregion

        #region Public API

        /// <summary>
        /// StatChangeInfo 데이터로 UI 설정
        /// 
        /// delta 값에 따라:
        /// - delta >= 0: 증가 표시 (녹색, 위 화살표, +N)
        /// - delta < 0:  감소 표시 (빨간색, 아래 화살표, -N)
        /// </summary>
        public void Setup(StatChangeInfo stat)
        {
            if (stat == null)
            {
                Debug.LogWarning("[StatUpDownComment] Stat change info is null");
                return;
            }

            // 1. 아이콘 설정
            SetStatIcon(stat.icon);

            // 2. 증가/감소 판별
            bool isIncrease = stat.delta >= 0;

            // 3. 화살표 표시 설정
            SetArrowVisibility(isIncrease);

            // 4. 텍스트 초기화
            ResetTextVisibility();

            // 5. 증감량 텍스트 설정
            SetDeltaText(stat.delta, isIncrease);

            Debug.Log($"[StatUpDownComment] Setup - delta: {stat.delta}, isIncrease: {isIncrease}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 스탯 아이콘 설정
        /// </summary>
        private void SetStatIcon(Sprite icon)
        {
            if (statIconImage != null && icon != null)
            {
                statIconImage.sprite = icon;
            }
        }

        /// <summary>
        /// 증가/감소 화살표 표시 설정
        /// </summary>
        private void SetArrowVisibility(bool isIncrease)
        {
            if (goUpImage != null)
                goUpImage.SetActive(isIncrease);

            if (goDownImage != null)
                goDownImage.SetActive(!isIncrease);
        }

        /// <summary>
        /// 모든 텍스트 숨김 (재설정 전 초기화)
        /// </summary>
        private void ResetTextVisibility()
        {
            if (valueSucsseText != null)
                valueSucsseText.gameObject.SetActive(false);

            if (valueFaillText != null)
                valueFaillText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 증감량 텍스트 설정
        /// 
        /// 표시 형식:
        /// - 증가: "+5" (녹색)
        /// - 감소: "−5" (빨간색, 마이너스 기호는 유니코드 U+2212)
        /// 
        /// 주의: "-" 대신 "−"를 사용하여 시각적 일관성 유지
        /// </summary>
        private void SetDeltaText(int delta, bool isIncrease)
        {
            string sign = isIncrease ? "+" : "−";  // U+2212 (minus sign), not hyphen
            int absoluteDelta = Mathf.Abs(delta);
            string displayText = $"{sign}{absoluteDelta}";

            if (isIncrease)
            {
                SetIncreaseText(displayText);
            }
            else
            {
                SetDecreaseText(displayText);
            }
        }

        /// <summary>
        /// 증가 텍스트 설정 (녹색)
        /// </summary>
        private void SetIncreaseText(string text)
        {
            if (valueSucsseText == null) return;

            valueSucsseText.text = text;
            valueSucsseText.color = upColor;
            valueSucsseText.gameObject.SetActive(true);
        }

        /// <summary>
        /// 감소 텍스트 설정 (빨간색)
        /// </summary>
        private void SetDecreaseText(string text)
        {
            if (valueFaillText == null) return;

            valueFaillText.text = text;
            valueFaillText.color = downColor;
            valueFaillText.gameObject.SetActive(true);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Inspector에서 필수 컴포넌트 검증
        /// </summary>
        private void OnValidate()
        {
            if (statIconImage == null)
                Debug.LogWarning("[StatUpDownComment] Stat icon image is not assigned", this);

            if (goUpImage == null)
                Debug.LogWarning("[StatUpDownComment] Up arrow image is not assigned", this);

            if (goDownImage == null)
                Debug.LogWarning("[StatUpDownComment] Down arrow image is not assigned", this);

            if (valueSucsseText == null)
                Debug.LogWarning("[StatUpDownComment] Success text is not assigned", this);

            if (valueFaillText == null)
                Debug.LogWarning("[StatUpDownComment] Fail text is not assigned", this);
        }

        #endregion
    }
}