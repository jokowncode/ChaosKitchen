
using System;
using TMPro;
using UnityEngine;

public class ScoreChangeTipMove : MonoBehaviour {

    [SerializeField] private float MoveSpeed = 100.0f;
    [SerializeField] private float UpDistance = 50.0f;
    
    private TextMeshProUGUI ScoreTipText;
    private RectTransform ScoreTipRectTransform;

    private Vector2 EndPosition;
    
    private void Awake() {
        ScoreTipText = this.GetComponent<TextMeshProUGUI>();
        ScoreTipRectTransform = this.GetComponent<RectTransform>();
    }

    public void UpdateScore(int score) {
        this.ScoreTipText.text = score > 0 ? "+" + score : score.ToString();
        this.ScoreTipText.color = score > 0 ? Color.green : Color.red;
        
        ScoreTipRectTransform.anchoredPosition = Vector2.up * 20.0f;
        EndPosition = ScoreTipRectTransform.anchoredPosition + Vector2.up * UpDistance;
    }

    private void Update() {
        if (ScoreTipRectTransform.anchoredPosition != EndPosition) {
            ScoreTipRectTransform.anchoredPosition = Vector2.MoveTowards(ScoreTipRectTransform.anchoredPosition, EndPosition, MoveSpeed * Time.deltaTime);
        } else {
            PoolManager.Instance.ReleaseScoreChangeUI(this);
        }
    }
}
