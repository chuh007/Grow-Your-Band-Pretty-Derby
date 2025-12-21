using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue Information", menuName = "SO/Dialogue/Information", order = 20)]
    public class DialogueInformationSO : ScriptableObject
    {
        [field: SerializeField] public List<Sprite> DialogueBackground { get; private set; }
        [field: SerializeField] public List<DialogueNode> DialogueDetails { get; private set; }
    }
}