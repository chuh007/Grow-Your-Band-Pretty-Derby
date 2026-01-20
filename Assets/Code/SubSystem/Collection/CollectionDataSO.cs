using System.Collections.Generic;
using Code.MainSystem.Outing;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    [CreateAssetMenu(fileName = "CollectionData", menuName = "SO/Collection/Data", order = 0)]
    public class CollectionDataSO : ScriptableObject
    {
        public string collectionName; // 소장품 이름
        public Sprite icon; // 소장품 이미지
        public bool haveCollection; // 이 소장품을 소유했는가?
        // 이거는 좀 생각해봐야할것같음
        // SO의 bool값은 데이터 조작이 매우 쉬운편일거라, 한다면 서버와 통신을 해서 검증하거나 해야함
        // 아니면 이거 말고 다른 방법이 있나?
        public int level; // 레벨? 사용할지 모름
        public int star; // 성급? 사용할지 모름. 에초에 int일지도 모르겠다
        public List<OutingEvent> plusEvents; // 이 소장품에 딸린 이벤트들
        
    }
}