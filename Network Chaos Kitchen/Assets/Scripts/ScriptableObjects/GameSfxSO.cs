
using UnityEngine;

[CreateAssetMenu(fileName = "GameSfx", menuName = "Chaos Kitchen/Game Sfx")]
public class GameSfxSO : ScriptableObject {
    public AudioClip[] ChopSounds;
    public AudioClip[] DeliveryFailSounds;
    public AudioClip[] DeliverySuccessSounds;
    public AudioClip[] ObjDropSounds;
    public AudioClip[] ObjPickupSounds;
    public AudioClip[] TrashSounds;
    public AudioClip[] WarningSounds;
    public AudioClip PanSizzleSound;
    public AudioClip TimeoutSound;
    public AudioClip DurCookingCompleteSound;
    public AudioClip SpraySound;
}
