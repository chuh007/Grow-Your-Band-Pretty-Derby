using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.Core.Bus.GameEvents.TraitEvents
{
    /// <summary>
    /// 특성 보유 현황 조회 요청에 대한 응답으로 사용되는 이벤트 
    /// </summary>
    public struct TraitShowResponded : IEvent
    {
        /// <summary>
        /// 조회된 활성 특성 목록
        /// </summary>
        public IReadOnlyList<ActiveTrait> ActiveTraits { get; }

        /// <summary>
        /// 특성 보유 현황 조회 결과 이벤트 생성자
        /// </summary>
        /// <param name="activeTraits">조회 대상 멤버가 현재 보유 중인 활성 특성 목록</param>
        public TraitShowResponded(IReadOnlyList<ActiveTrait> activeTraits)
        {
            ActiveTraits = activeTraits;
        }
    }
}