
using System;
using UnityEngine;

public class SpriteManager : MonoBehaviour {
    
    [Header("Sprite")] 
    [SerializeField] private GameSpriteSO GameSprites;

    public static SpriteManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public Sprite GetCookOpSprite(CookOP cookOP) {
        switch (cookOP) {
            case CookOP.Cut:
                return GameSprites.CutSprite;
            case CookOP.Fry:
                return GameSprites.FrySprite;
            case CookOP.DeepFry:
                return GameSprites.DeepFrySprite;
            case CookOP.Boil:
                return GameSprites.BoilSprite;
        }
        return null;
    }

    public Sprite GetLockSprite(bool locked) {
        return locked ? GameSprites.LockedSprite : GameSprites.UnlockedSprite;
    }
}

