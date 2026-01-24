using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.MainScreen.Training
{
    public class CommentManager : MonoBehaviour
    {
        public static CommentManager instance;

        [SerializeField] private PracticeCommentPage commentPage;

        private readonly List<CommentData> _pendingComments = new();
        private string _name;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddComment(CommentData data,string name)
        {
            _pendingComments.Add(data);
            _name = name;
        }

        public async UniTask ShowAllComments()
        {
            if (_pendingComments.Count == 0) return;

            await commentPage.ShowComments(_pendingComments,_name);
            _pendingComments.Clear();
        }
    }
}