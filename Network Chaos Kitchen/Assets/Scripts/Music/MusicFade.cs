
using System;
using System.Collections;
using UnityEngine;

public class MusicFade : MonoBehaviour {

    private AudioSource MusicSource;

    public bool IsPlay => MusicSource.isPlaying;
    
    public void SetVolume(float value) {
        StopAllCoroutines();
        MusicSource.volume = value;
    }

    private void Awake() {
        MusicSource = GetComponent<AudioSource>();
    }

    public void FadeIn(float targetVolume, float duration = 1.0f) {
        StopAllCoroutines();
        StartCoroutine(Fade(MusicSource.volume, targetVolume, duration));
    }

    public void FadeOut(float duration = 1.0f) {
        StopAllCoroutines();
        StartCoroutine(Fade(MusicSource.volume, 0.0f, duration));
    }

    private IEnumerator Fade(float start, float end, float duration = 1.0f) {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration) {
            MusicSource.volume = Mathf.Lerp(start, end, t);
            yield return null;
        }
    }

    public void PlayMusic() {
        this.MusicSource.Play();        
    }
}
