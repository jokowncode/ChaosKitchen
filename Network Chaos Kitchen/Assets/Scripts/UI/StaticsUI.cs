using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaticsUI : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI FinalScoreText;
    [SerializeField] private TextMeshProUGUI OrderCompleteText;
    [SerializeField] private Button NextLevelButton;
    [SerializeField] private Image[] StarImage;
    [SerializeField] private GameObject HostButtons;
    
    private Animator StaticsAnimator;
    
    private void Awake() {
        StaticsAnimator = GetComponent<Animator>();
    }

    public void ShowStaticsUI(LevelInfoSO levelInfo, int score, int orderComplete) {
        this.FinalScoreText.text = score.ToString();
        this.OrderCompleteText.text = orderComplete.ToString();

        StarImage[0].color = score >= levelInfo.PrimaryScore ? Color.white : Color.black;
        StarImage[1].color = score >= levelInfo.IntermediateScore ? Color.white : Color.black;
        StarImage[2].color = score >= levelInfo.SeniorScore ? Color.white : Color.black;
        
        this.HostButtons.SetActive(NetworkGameManager.Instance.IsHost);
        this.NextLevelButton.gameObject.SetActive(score >= levelInfo.PrimaryScore && levelInfo.NextLevel != Level.StartGameLoading);
        StaticsAnimator.SetBool(AnimationParams.Show, true);
    }

    public void HideStaticsUI() {
        StaticsAnimator.SetBool(AnimationParams.Show, false);
    }
}
