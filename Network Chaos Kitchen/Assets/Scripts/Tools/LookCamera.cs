
using System;
using UnityEngine;

public class LookCamera : MonoBehaviour {

    [SerializeField] private bool Invert = true;

    private void LateUpdate() {
        int sign = Invert ? -1 : 1;
        this.transform.forward = sign * Camera.main.transform.forward;
    }
}
