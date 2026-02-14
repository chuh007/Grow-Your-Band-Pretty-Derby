using System;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.SubSystem.Lobby.Album.Data
{
    [Serializable]
    public class MemberPart
    {
        public MemberType memberType;
        public AudioClip partClip;
    }
    
    [CreateAssetMenu(fileName = "Album", menuName = "SO/Album/Data", order = 0)]
    public class AlbumDataSO : ScriptableObject
    {
        public AudioClip FirstSong;
        public AudioClip SecondSong;
        public Sprite AlbumImage;
        public List<MemberPart> FirstSongMemberPart;
        public List<MemberPart> SecondSongMemberPart;
        public AudioClip FirstSongHighlight;
        public AudioClip SecondSongHighlight;
        
    }
}