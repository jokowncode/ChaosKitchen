
using System;
using UnityEngine;

public class MainMusicManager : MonoBehaviour {

    public static MainMusicManager Instance { get; private set; }
    
    private MusicFade MainMusic;
    public float CurrentMainMusicVolume { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        MainMusic = GetComponent<MusicFade>();
    }
    
    public void ChangeMainMusicVolume(float value) {
        this.MainMusic.SetVolume(value);
        this.CurrentMainMusicVolume = value;
    }

    public void MainMusicFadeIn(float duration = 1.0f) {
        this.MainMusic.FadeIn(this.CurrentMainMusicVolume, duration);
    }

    public void MainMusicFadeOut(float duration = 1.0f) {
        this.MainMusic.FadeOut(duration);        
    }

    public void SetMainMusicVolume(float value) {
        this.CurrentMainMusicVolume = value;
        this.MainMusic.SetVolume(value);
    }

    public void PlayMainMusic() {
        if (!this.MainMusic.IsPlay) {
            this.MainMusic.PlayMusic();
        }
    }
}


