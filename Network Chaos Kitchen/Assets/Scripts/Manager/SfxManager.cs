using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SfxManager : MonoBehaviour {

    [SerializeField] private GameSfxSO GameSfx;

    public float VolumeMultiplier { get; private set; } = 1.0f;

    public static SfxManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SfxVolumeChange(float value) {
        this.VolumeMultiplier = value;
    }

    private static AudioClip GetRandomClip(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

    private static AudioClip GetRandomClip(AudioClip clip) {
        return clip;
    }

    private AudioClip GetAudioClip(SFXType type) {
        return type switch {
            SFXType.Chop => GetRandomClip(this.GameSfx.ChopSounds),
            SFXType.DeliveryFail => GetRandomClip(this.GameSfx.DeliveryFailSounds),
            SFXType.DeliverySuccess => GetRandomClip(this.GameSfx.DeliverySuccessSounds),
            SFXType.ObjDrop => GetRandomClip(this.GameSfx.ObjDropSounds),
            SFXType.ObjPickup => GetRandomClip(this.GameSfx.ObjPickupSounds),
            SFXType.Trash => GetRandomClip(this.GameSfx.TrashSounds),
            SFXType.Warning => GetRandomClip(this.GameSfx.WarningSounds),
            SFXType.PanSizzle => GetRandomClip(this.GameSfx.PanSizzleSound),
            SFXType.Timeout => GetRandomClip(this.GameSfx.TimeoutSound),
            SFXType.DurCookingComplete => GetRandomClip(this.GameSfx.DurCookingCompleteSound),
            SFXType.Spray => GetRandomClip(this.GameSfx.SpraySound),
            _ => null
        };
    }

    public void PlaySound(SFXType type, Vector3 position, float volume = 1.0f) {
        AudioSource.PlayClipAtPoint(GetAudioClip(type), position, volume * VolumeMultiplier);
    }

    public void PlayOneShot(AudioSource audioSource, SFXType type, float volume = 1.0f) {
        audioSource.PlayOneShot(GetAudioClip(type), volume * this.VolumeMultiplier);
    }

    public void PlaySound(AudioSource audioSource, SFXType type, float volume = 1.0f) {
        audioSource.clip = GetAudioClip(type);
        audioSource.volume = volume * this.VolumeMultiplier;
        audioSource.Play();
    }
}
