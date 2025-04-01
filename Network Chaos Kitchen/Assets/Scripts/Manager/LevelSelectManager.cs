using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField] private Transform LevelUIParent;
    [SerializeField] private LevelUI LevelUIPrefab;

    private void Start() {
        LevelInfoSO[] levelInfos = GameManager.Instance.LevelInfos;
        foreach (LevelInfoSO levelInfo in levelInfos) {
            LevelUI levelUI = Instantiate(LevelUIPrefab, LevelUIParent);
            levelUI.SetLevelInfo(levelInfo);
        }
    }

    public void GoToMainMenu() {
        GameManager.Instance.GoBackToMainMenu();
    }
}

