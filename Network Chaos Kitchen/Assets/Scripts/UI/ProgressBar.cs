using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    
    [SerializeField] private Image ProgressImage;
    
    private void Awake() {
        UpdateProgress(1.0f);
    }
    
    public void SetWorldPosition(Vector3 worldPos) {
        this.transform.SetIndicatorUIPosition(worldPos);
    }
    
    public void Show() {
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }

    public void UpdateProgress(float value) {
        ProgressImage.fillAmount = value;
    }
}
