
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI LevelName;
    [SerializeField] private TextMeshProUGUI LevelScore;
    [SerializeField] private Image[] StarImage;
    [SerializeField] private Button StartButton;
    [SerializeField] private Image StartImage;

    public void SetLevelInfo(LevelInfoSO levelInfo) {
        int levelScore = GameManager.Instance.GetLevelScore(levelInfo.CurrentLevel);
        bool locked = GameManager.Instance.LevelIsLocked(levelInfo.CurrentLevel);
        
        LevelName.text = levelInfo.LevelName;
        LevelScore.text = levelScore.ToString();

        if (levelScore >= levelInfo.PrimaryScore) {
            StarImage[0].color = Color.white;
        }
        
        if (levelScore >= levelInfo.IntermediateScore) {
            StarImage[1].color = Color.white;
        }

        if (levelScore >= levelInfo.SeniorScore) {
            StarImage[2].color = Color.white;
        }
        
        StartImage.sprite = SpriteManager.Instance.GetLockSprite(locked);
        StartButton.interactable = !locked;
        StartButton.onClick.AddListener(() => {
            NetworkGameManager.Instance.GoToLevelClientRpc(levelInfo.CurrentLevel);
        });
    }
}


