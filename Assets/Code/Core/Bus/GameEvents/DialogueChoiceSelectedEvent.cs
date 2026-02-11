using System.Collections.Generic;
using Code.MainSystem.Dialogue.DialogueEvent;
using UnityEngine.AddressableAssets;

namespace Code.Core.Bus.GameEvents
{
    /// <summary>
    /// 플레이어가 선택지를 선택했을 때 발생하는 이벤트
    /// </summary>
    public struct DialogueChoiceSelectedEvent : IEvent
    {
        /// <summary>
        /// 다음에 이동할 노드 인덱스
        /// </summary>
        public readonly int NextNodeIndex;

        /// <summary>
        /// 선택에 따른 추가 이벤트 리스트
        /// </summary>
        public readonly List<AssetReferenceT<BaseDialogueEventSO>> Events;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="nextNodeIndex">다음 노드 인덱스</param>
        /// <param name="events">발생할 이벤트 리스트</param>
        public DialogueChoiceSelectedEvent(int nextNodeIndex, List<AssetReferenceT<BaseDialogueEventSO>> events)
        {
            NextNodeIndex = nextNodeIndex;
            Events = events;
        }
    }
}