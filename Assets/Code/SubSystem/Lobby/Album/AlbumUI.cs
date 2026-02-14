using System;
using System.Collections.Generic;
using Code.SubSystem.Lobby.Album.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Code.SubSystem.Lobby.Album
{
    [Serializable]
    public class AlbumListData
    {
        public int Id;
        public AlbumDataSO Albums;
    }
    
    public class AlbumUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Data")]
        [SerializeField] private List<AlbumListData> albums;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private string gameSceneName;
        
        [Header("UI")] 
        [SerializeField] private GameObject panel;
        [SerializeField] private Image albumImage;
        [SerializeField] private Button playSong1Button;
        [SerializeField] private Button playSong2Button;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("Album Navigation")]
        [SerializeField] private Button prevAlbumButton;
        [SerializeField] private Button nextAlbumButton;
        [SerializeField] private TextMeshProUGUI albumIndexText;
        [SerializeField] private float swipeThreshold = 50f; 
        
        private AlbumDataSO _currentAlbum;
        private int _currentAlbumIndex = 0;
        private int _currentSongIndex = -1;
        private bool _isPaused = false;
        
        private Vector2 _dragStartPos;
        private bool _isDragging = false;

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.volume = 1f; 
            
            if (playSong1Button != null)
            {
                playSong1Button.onClick.AddListener(() => {
                    PlaySong(0);
                });
            }
            
            if (playSong2Button != null)
            {
                playSong2Button.onClick.AddListener(() => {
                    PlaySong(1);
                });
            }
            
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(TogglePause);
            }
            
            if (stopButton != null)
            {
                stopButton.onClick.AddListener(StopSong);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }
            
            if (progressSlider != null)
            {
                progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnGameStart);
            }
            
            if (prevAlbumButton != null)
            {
                prevAlbumButton.onClick.AddListener(ShowPreviousAlbum);
            }
            
            if (nextAlbumButton != null)
            {
                nextAlbumButton.onClick.AddListener(ShowNextAlbum);
            }
            
            if (albums.Count > 0)
            {
                SelectAlbum(0);
            }
            
            UpdateUI();
            UpdateAlbumNavigationUI();
            
            panel.SetActive(false);
        }

        private void OnGameStart()
        {
            AlbumManager.Instance.SetAlbumData(_currentAlbum);
            StopSong();
            SceneManager.LoadScene(gameSceneName);
        }

        private void ClosePanel()
        {
            panel.gameObject.SetActive(false);
        }

        public void OpenPanel()
        {
            panel.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (audioSource.isPlaying && audioSource.clip != null)
            {
                UpdateProgressBar();
            }
            
            if (_currentSongIndex != -1 && !audioSource.isPlaying && !_isPaused)
            {
                StopSong();
            }
        }
        
        #region Album Navigation
        
        /// <summary>
        /// 인덱스로 앨범 선택
        /// </summary>
        private void SelectAlbum(int index)
        {
            if (index < 0 || index >= albums.Count)
                return;
                
            _currentAlbumIndex = index;
            _currentAlbum = albums[_currentAlbumIndex].Albums;
            
            StopSong();
            
            if (albumImage != null && _currentAlbum != null && _currentAlbum.AlbumImage != null)
            {
                albumImage.sprite = _currentAlbum.AlbumImage;
            }
            
            UpdateAlbumNavigationUI();
            Debug.Log($"[AlbumUI] Selected album {_currentAlbumIndex + 1}/{albums.Count}");
        }
        
        /// <summary>
        /// 이전 앨범으로 이동
        /// </summary>
        public void ShowPreviousAlbum()
        {
            if (albums.Count <= 1) return;
            
            int newIndex = _currentAlbumIndex - 1;
            if (newIndex < 0)
                newIndex = albums.Count - 1; 
            
            SelectAlbum(newIndex);
        }
        
        /// <summary>
        /// 다음 앨범으로 이동
        /// </summary>
        public void ShowNextAlbum()
        {
            if (albums.Count <= 1) return;
            
            int newIndex = _currentAlbumIndex + 1;
            if (newIndex >= albums.Count)
                newIndex = 0; 
            
            SelectAlbum(newIndex);
        }
        
        /// <summary>
        /// 앨범 네비게이션 UI 업데이트 (버튼 활성화, 인덱스 표시)
        /// </summary>
        private void UpdateAlbumNavigationUI()
        {
            if (albumIndexText != null)
            {
                albumIndexText.text = $"{_currentAlbumIndex + 1} / {albums.Count}";
            }
            
            bool hasMultipleAlbums = albums.Count > 1;
            
            if (prevAlbumButton != null)
            {
                prevAlbumButton.gameObject.SetActive(hasMultipleAlbums);
            }
            
            if (nextAlbumButton != null)
            {
                nextAlbumButton.gameObject.SetActive(hasMultipleAlbums);
            }
        }
        
        #endregion
        
        #region Swipe Handling
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStartPos = eventData.position;
            _isDragging = true;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _isDragging = false;
            
            Vector2 dragEndPos = eventData.position;
            Vector2 dragDelta = dragEndPos - _dragStartPos;
            
            if (Mathf.Abs(dragDelta.x) > swipeThreshold && Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
            {
                if (dragDelta.x > 0)
                {
                    ShowPreviousAlbum();
                    Debug.Log("[AlbumUI] Swiped right - Previous album");
                }
                else
                {
                    ShowNextAlbum();
                    Debug.Log("[AlbumUI] Swiped left - Next album");
                }
            }
        }
        
        #endregion
        
        private void PlaySong(int songIndex)
        {
            if (_currentAlbum == null)
            {
                return;
            }
            
            AudioClip clipToPlay = null;
            
            if (songIndex == 0)
            {
                clipToPlay = _currentAlbum.FirstSong;
            }
            else if (songIndex == 1)
            {
                clipToPlay = _currentAlbum.SecondSong;
            }
            
            if (clipToPlay == null)
            {
                return;
            }
            
            if (_currentSongIndex == songIndex)
            {
                audioSource.Stop();
                audioSource.time = 0;
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = clipToPlay;
            }
            
            audioSource.Play();
            
            _currentSongIndex = songIndex;
            _isPaused = false;
            
            UpdateUI();
        }
        
        private void TogglePause()
        {
            if (_currentSongIndex == -1)
            {
                return;
            }
            
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                _isPaused = true;
            }
            else if (_isPaused)
            {
                audioSource.UnPause();
                _isPaused = false;
            }
            
            UpdateUI();
        }
        
        private void StopSong()
        {
            audioSource.Stop();
            _currentSongIndex = -1;
            _isPaused = false;
            
            if (progressSlider != null)
            {
                progressSlider.value = 0;
            }
            
            UpdateUI();
        }
        
        private void UpdateProgressBar()
        {
            if (audioSource.clip == null) return;
            
            float progress = audioSource.time / audioSource.clip.length;
            
            if (progressSlider != null)
            {
                progressSlider.onValueChanged.RemoveListener(OnProgressSliderChanged);
                progressSlider.value = progress;
                progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
            }
            
            if (timeText != null)
            {
                string currentTime = FormatTime(audioSource.time);
                string totalTime = FormatTime(audioSource.clip.length);
                timeText.text = $"{currentTime} / {totalTime}";
            }
        }
        
        private void OnProgressSliderChanged(float value)
        {
            if (_currentSongIndex != -1 && audioSource.clip != null)
            {
                audioSource.time = value * audioSource.clip.length;
            }
        }
        
        private string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            return $"{minutes:00}:{seconds:00}";
        }
        
        private void UpdateUI()
        {
            if (playSong1Button != null)
            {
                var colors = playSong1Button.colors;
                colors.normalColor = (_currentSongIndex == 0) ? Color.green : Color.white;
                playSong1Button.colors = colors;
            }
            
            if (playSong2Button != null)
            {
                var colors = playSong2Button.colors;
                colors.normalColor = (_currentSongIndex == 1) ? Color.green : Color.white;
                playSong2Button.colors = colors;
            }
            
            if (pauseButton != null)
            {
                var buttonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = _isPaused ? "재개" : "일시정지";
                }
                pauseButton.interactable = (_currentSongIndex != -1);
            }
            
            if (stopButton != null)
            {
                stopButton.interactable = (_currentSongIndex != -1);
            }
        }
        
        private void OnDestroy()
        {
            if (playSong1Button != null) playSong1Button.onClick.RemoveAllListeners();
            if (playSong2Button != null) playSong2Button.onClick.RemoveAllListeners();
            if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
            if (stopButton != null) stopButton.onClick.RemoveAllListeners();
            if (closeButton != null) closeButton.onClick.RemoveAllListeners();
            if (nextButton != null) nextButton.onClick.RemoveAllListeners();
            if (progressSlider != null) progressSlider.onValueChanged.RemoveAllListeners();
            
            if (prevAlbumButton != null) prevAlbumButton.onClick.RemoveAllListeners();
            if (nextAlbumButton != null) nextAlbumButton.onClick.RemoveAllListeners();
        }
    }
}