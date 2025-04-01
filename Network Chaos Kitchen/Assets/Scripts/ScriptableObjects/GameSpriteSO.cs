
using UnityEngine;

[CreateAssetMenu(fileName = "GameSprites", menuName = "Chaos Kitchen/Game Sprite")]
public class GameSpriteSO : ScriptableObject {
    [Header("Cooking Operation Sprite")]
    public Sprite FrySprite;
    public Sprite CutSprite;
    public Sprite DeepFrySprite;
    public Sprite BoilSprite;
    
    [Header("Level Choose Sprite")]
    public Sprite LockedSprite;
    public Sprite UnlockedSprite;
}
