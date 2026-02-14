using System;
using System.Collections.Generic;
using Code.SubSystem.Lobby.Album.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Code.SubSystem.Lobby.Album
{
    [Serializable]
    public class AlbumListData
    {
        public int Id;
        public AlbumDataSO Albums;
    }
    
    public class AlbumUI : MonoBehaviour
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
        
        private AlbumDataSO _currentAlbum;
        private int _currentSongIndex = -1;
        private bool _isPaused = false;

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
            
            if (albums.Count > 0)
            {
                SelectAlbum(albums[0].Id);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(OnGameStart);
            }
            
            UpdateUI();
            
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
        
        public void SelectAlbum(int albumId)
        {
            
            _currentAlbum = albums.Find(a => a.Id == albumId)?.Albums;
            
            StopSong();
        }
        
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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
            
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
            }
            
            if (progressSlider != null)
            {
                progressSlider.onValueChanged.RemoveAllListeners();
            }
        }
    }
}