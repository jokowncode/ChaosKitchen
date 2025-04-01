
using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColorUI : MonoBehaviour {

    [SerializeField] private Image ColorImage;
    [SerializeField] private GameObject Selected;
    [SerializeField] private Button ColorButton;

    private int ColorIndex;
    public Action<int> OnSelected;

    private void Awake() {
        ColorButton.onClick.AddListener(() => {
            if (Selected.activeInHierarchy) return;
            OnSelected?.Invoke(this.ColorIndex);
        });
    }

    public void SetColor(Color color, int colorIndex) {
        this.ColorIndex = colorIndex;
        ColorImage.color = color;
    }

    public void ToggleSelected(bool show) {
        Selected.SetActive(show);
    }
}


