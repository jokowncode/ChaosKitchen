
using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {

    [SerializeField] private MeshRenderer HeadRenderer;
    [SerializeField] private MeshRenderer BodyRenderer;

    private Material PlayerMaterial;

    private void Awake() {
        PlayerMaterial = new Material(HeadRenderer.material);
        HeadRenderer.material = PlayerMaterial;
        BodyRenderer.material = PlayerMaterial;
    }

    public void SetColor(Color color) {
        PlayerMaterial.color = color;
    }

}


