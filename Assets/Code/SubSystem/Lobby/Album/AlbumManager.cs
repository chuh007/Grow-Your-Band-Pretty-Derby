using System;
using Code.MainSystem.StatSystem.Manager;
using Code.SubSystem.Lobby.Album.Data;
using UnityEngine;

namespace Code.SubSystem.Lobby.Album
{
    public class AlbumManager : MonoBehaviour
    {
        public static AlbumManager Instance;

        private AlbumDataSO _currentAlbum;
        private int _currentSong = 1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public void SetAlbumData(AlbumDataSO data)
        {
            _currentAlbum = data;
        }

        /// <summary>
        ///   앨범 데이터 가져올때 쓰셈
        /// </summary>
        public AlbumDataSO GetCurrentAlbum()
        {
            return _currentAlbum;
        }

        /// <summary>
        ///  멤버 파트만 가져오고싶을때 맴버 타입보내주면됨
        /// </summary>
        public AudioClip GetMemberPartAudio(MemberType memberType)
        {
            if (_currentSong == 1)
            {
                foreach (var data in _currentAlbum.FirstSongMemberPart)
                {
                    if (data.memberType == memberType)
                    {
                        return data.partClip;
                    }
                }
            }
            else if (_currentSong == 2)
            {
                foreach (var data in _currentAlbum.SecondSongMemberPart)
                {
                    if (data.memberType == memberType)
                    {
                        return data.partClip;
                    }
                }
            }
            return null;
        }
        
        /// <summary>
        /// 다음곡으로 넘어갈때 쓰는함수
        /// </summary>
        public void NextSong()
        {
            _currentSong++;
        }
        
        /// <summary>
        /// 게임 끝나고 초기화할때 쓸것들
        /// </summary>
        public void RestartSong()
        {
            _currentSong = 1;
            _currentAlbum = null;
        }
    }
}