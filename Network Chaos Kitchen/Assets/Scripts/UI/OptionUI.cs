
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour {

    [Header("Audio Options")]
    [SerializeField] private Slider MainMusicVolumeSlider;
    [SerializeField] private TextMeshProUGUI MainMusicVolumeText;
    [SerializeField] private Slider SfxMusicVolumeSlider;
    [SerializeField] private TextMeshProUGUI SfxVolumeText;

    [Header("InputOptions")] 
    [SerializeField] private Button MoveUpInputButton;
    [SerializeField] private Button MoveDownInputButton;
    [SerializeField] private Button MoveLeftInputButton;
    [SerializeField] private Button MoveRightInputButton;
    [SerializeField] private Button InteractInputButton;
    [SerializeField] private Button InteractAltInputButton;
    [SerializeField] private Button PauseInputButton;
    [SerializeField] private Button RunInputButton;
    [SerializeField] private TextMeshProUGUI MoveUpInputText;
    [SerializeField] private TextMeshProUGUI MoveDownInputText;
    [SerializeField] private TextMeshProUGUI MoveLeftInputText;
    [SerializeField] private TextMeshProUGUI MoveRightInputText;
    [SerializeField] private TextMeshProUGUI InteractInputText;
    [SerializeField] private TextMeshProUGUI InteractAltInputText;
    [SerializeField] private TextMeshProUGUI PauseInputText;
    [SerializeField] private TextMeshProUGUI RunInputText;
    
    private Animator OptionAnimator;
    
    private void Awake() {
        OptionAnimator = this.GetComponent<Animator>();
        
        MoveUpInputButton.onClick.AddListener(() => {
            RebindInput(InputType.MoveUp);
        });
        MoveDownInputButton.onClick.AddListener(() => {
            RebindInput(InputType.MoveDown);
        });
        MoveLeftInputButton.onClick.AddListener(() => {
            RebindInput(InputType.MoveLeft);
        });
        MoveRightInputButton.onClick.AddListener(() => {
            RebindInput(InputType.MoveRight);
        });
        InteractInputButton.onClick.AddListener(() => {
            RebindInput(InputType.Interact);
        });
        InteractAltInputButton.onClick.AddListener(() => {
            RebindInput(InputType.InteractAlt);
        });
        PauseInputButton.onClick.AddListener(() => {
            RebindInput(InputType.Pause);
        });
        RunInputButton.onClick.AddListener(() => {
            RebindInput(InputType.Run);
        });
    }

    public void ShowOption(float mainMusicVolume) {
        OptionAnimator.SetBool(AnimationParams.Show, true);
        
        MainMusicVolumeText.text = Mathf.RoundToInt(mainMusicVolume * 100).ToString();
        MainMusicVolumeSlider.value = mainMusicVolume;

        float sfxVolume = SfxManager.Instance.VolumeMultiplier;
        SfxMusicVolumeSlider.value = sfxVolume;
        SfxVolumeText.text = Mathf.RoundToInt(sfxVolume * 100).ToString();

        InputSetting setting = InputManager.Instance.GetCurrentInputSetting();
        MoveUpInputText.text = setting.MoveUp;
        MoveDownInputText.text = setting.MoveDown;
        MoveLeftInputText.text = setting.MoveLeft;
        MoveRightInputText.text = setting.MoveRight;
        InteractInputText.text = setting.Interact;
        InteractAltInputText.text = setting.InteractAlt;
        PauseInputText.text = setting.Pause;
        RunInputText.text = setting.Run;
    }

    private TextMeshProUGUI GetInputTextMeshProGUI(InputType type) {
        return type switch {
            InputType.MoveUp => this.MoveUpInputText,
            InputType.MoveDown => this.MoveDownInputText,
            InputType.MoveLeft => this.MoveLeftInputText,
            InputType.MoveRight => this.MoveRightInputText,
            InputType.Interact => this.InteractInputText,
            InputType.InteractAlt => this.InteractAltInputText,
            InputType.Pause => this.PauseInputText,
            InputType.Run => this.RunInputText,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private int GetInputBindingIndex(InputType type) {
        return type switch {
            InputType.MoveUp => 1,
            InputType.MoveDown => 2,
            InputType.MoveLeft => 3,
            InputType.MoveRight => 4,
            InputType.Interact => 0,
            InputType.InteractAlt => 0,
            InputType.Pause => 0,
            InputType.Run => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private void RebindInput(InputType type) {
        TextMeshProUGUI rebind = GetInputTextMeshProGUI(type);
        int bindingIndex = GetInputBindingIndex(type);
        rebind.text = "...";
        InputManager.Instance.RebindInput(type, callback => {
            rebind.text = callback.action.bindings[bindingIndex].ToDisplayString();
        });
    }

    public void ChangeMainMusicVolume() {
        MainMusicVolumeText.text = Mathf.RoundToInt(MainMusicVolumeSlider.normalizedValue * 100).ToString();
        MainMusicManager.Instance.ChangeMainMusicVolume(MainMusicVolumeSlider.normalizedValue);
    }

    public void ChangeSfxVolume() {
        SfxVolumeText.text = Mathf.RoundToInt(SfxMusicVolumeSlider.normalizedValue * 100).ToString();
        SfxManager.Instance.SfxVolumeChange(SfxMusicVolumeSlider.normalizedValue);
    }

    public void HideOption() {
        OptionAnimator.SetBool(AnimationParams.Show, false);
    }
}
