using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.Etc
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "SO/TMI/Data", order = 0)]
    public class TMIListSO : ScriptableObject
    {
        [TextArea]
        public List<String> TMIList = new List<string>();
    }
}