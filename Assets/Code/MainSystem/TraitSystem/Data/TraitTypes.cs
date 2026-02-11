using System;

namespace Code.MainSystem.TraitSystem.Data
{
    public enum TraitType
    {
        NoneTrait,              // 특성 없음
        Telepathy,              // 이심전심
        LoneGuitarist,          // 고독한 기타리스트
        ShiningEyes,            // 반짝이는 눈
        FailureBreedsSuccess,   // 실패는 성공의 어머니
        HonedTechnique,         // 단련기술
        Injury,                 // 부상
        Overzealous,            // 지나친 열정
        Dogmatic,               // 독선적
        Entertainer,            // 만담가
        Focus,                  // 집중력
        HighlightBoost,         // 하이라이트 강화
        AttentionGain,          // 주목도 상승
        BreathControl,          // 호흡 조절
    }

    [Flags]
    public enum TraitTag
    {
        None = 0,           // 특성 태그 없음
        Teamwork = 1 << 0,  // 팀워크	
        Support = 1 << 1,   // 백업
        Stability = 1 << 2, // 안정감
        Energy = 1 << 3,    // 텐션 업
        Genius = 1 << 4,    // 천재
        Solo = 1 << 5,      // 독주
        Mastery = 1 << 6,   // 극한 연습
        Immersion = 1 << 7, // 몰입
        GuitarSolo = 1 << 8,// 기타 솔로
    }

    public enum TraitTarget
    {
        None,
        Ensemble,           // 합주 효과 관련
        EnsembleCondition,  // 합주 컨디션 관련
        PracticeCondition,  // 개인 연습 컨디션 관련
        PracticeMental,     // 개인 연습 멘탈 관련
        Practice,           // 개인 연습 효과 관련
        Condition,          // 모든 컨디션 소모/변화 관련
        SuccessRate,        // 훈련/합주 성공률 관련
        Training,           // 능력치 상승 효율 관련
        Mental,             // 멘탈 능력치 관련
        FeverScore,         // 피버 점수 관련
        FeverTime,          // 피버 지속시간 관련
        FeverInput,         // 피버 필요 입력 관련
        Harmony,           // 하모니 관련
        
    }

    public enum CalculationType
    {
        Additive,       // + (고정치 증가)
        Subtractive,    // - (고정치 감소)
        PercentAdd,     // +% (퍼센트 합연산 증가)
        PercentSub,     // -% (퍼센트 합연산 감소)
        Multiplicative  // x (최종 곱연산, 보통 축복/디버프 등 독립 계산용)
    }
}