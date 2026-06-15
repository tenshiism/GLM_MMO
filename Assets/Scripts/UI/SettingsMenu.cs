using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private enum Tab { Audio, Controls, Video }

    [Header("Tab Buttons")]
    [SerializeField] private Button audioTabButton;
    [SerializeField] private Button controlsTabButton;
    [SerializeField] private Button videoTabButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject videoPanel;

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValueText;
    [SerializeField] private Transform keybindContainer;
    [SerializeField] private GameObject keybindRowPrefab;

    [Header("Video")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TMP_Text fovValueText;
    [SerializeField] private Slider renderScaleSlider;
    [SerializeField] private TMP_Text renderScaleValueText;
    [SerializeField] private TMP_Dropdown shadowQualityDropdown;
    [SerializeField] private Slider shadowDistanceSlider;
    [SerializeField] private TMP_Text shadowDistanceValueText;
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;
    [SerializeField] private TMP_Dropdown textureQualityDropdown;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    private Resolution[] resolutions;
    private Tab currentTab = Tab.Audio;
    private bool rebindingAction;
    private KeybindManager.Action rebindingWhat;
    private TMP_Text rebindLabel;
    private float rebindTimeout;

    private static readonly KeybindManager.Action[] rebindableActions = new KeybindManager.Action[]
    {
        KeybindManager.Action.MoveForward,
        KeybindManager.Action.MoveBack,
        KeybindManager.Action.StrafeLeft,
        KeybindManager.Action.StrafeRight,
        KeybindManager.Action.Jump,
        KeybindManager.Action.Fire,
        KeybindManager.Action.AltFire,
        KeybindManager.Action.Reload,
        KeybindManager.Action.Interact,
        KeybindManager.Action.UseItem,
        KeybindManager.Action.Ping,
        KeybindManager.Action.Slot1,
        KeybindManager.Action.Slot2,
        KeybindManager.Action.Slot3,
        KeybindManager.Action.Sprint,
        KeybindManager.Action.Crouch,
        KeybindManager.Action.OpenMenu,
        KeybindManager.Action.OpenInventory
    };

    private static readonly string[] rebindableNames = new string[]
    {
        "Move Forward", "Move Back", "Strafe Left", "Strafe Right",
        "Jump", "Fire", "Alt Fire", "Reload", "Interact", "Use Item",
        "Ping", "Slot 1", "Slot 2", "Slot 3", "Sprint", "Crouch",
        "Open Menu", "Open Inventory"
    };

    private void OnEnable()
    {
        LoadSettings();
        if (currentTab == Tab.Controls)
            RefreshKeybindUI();
    }

    private void Start()
    {
        PopulateResolutions();
        PopulateQualityLevels();
        PopulateShadowQuality();
        PopulateAntiAliasing();
        PopulateTextureQuality();
        BindListeners();
        BuildKeybindUI();
        SwitchTab(Tab.Audio);
        LoadSettings();
    }

    private void Update()
    {
        if (rebindingAction)
        {
            rebindTimeout -= Time.deltaTime;
            if (rebindTimeout <= 0f || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelRebind();
                return;
            }

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (key == KeyCode.None) continue;
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.Escape)
                    {
                        CancelRebind();
                        return;
                    }
                    KeybindManager.Rebind(rebindingWhat, key);
                    rebindingAction = false;
                    RefreshKeybindUI();
                    return;
                }
            }
        }
    }

    private void PopulateResolutions()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        int selected = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            string label = $"{r.width} x {r.height} @ {r.refreshRateRatio.value:F0}Hz";
            options.Add(label);
            if (r.width == Screen.currentResolution.width &&
                r.height == Screen.currentResolution.height)
                selected = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = selected;
        resolutionDropdown.RefreshShownValue();
    }

    private void PopulateQualityLevels()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void PopulateShadowQuality()
    {
        if (shadowQualityDropdown == null) return;
        shadowQualityDropdown.ClearOptions();
        shadowQualityDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Off", "Low", "Medium", "High"
        });
        shadowQualityDropdown.value = SettingsManager.shadowQuality;
        shadowQualityDropdown.RefreshShownValue();
    }

    private void PopulateAntiAliasing()
    {
        if (antiAliasingDropdown == null) return;
        antiAliasingDropdown.ClearOptions();
        antiAliasingDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Off", "2x MSAA", "4x MSAA", "8x MSAA"
        });
        antiAliasingDropdown.RefreshShownValue();
    }

    private void PopulateTextureQuality()
    {
        if (textureQualityDropdown == null) return;
        textureQualityDropdown.ClearOptions();
        textureQualityDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Full", "Half", "Quarter", "Eighth"
        });
        textureQualityDropdown.RefreshShownValue();
    }

    private void BindListeners()
    {
        if (applyButton != null) applyButton.onClick.AddListener(OnApply);
        if (resetButton != null) resetButton.onClick.AddListener(OnReset);
        if (backButton != null) backButton.onClick.AddListener(OnBack);

        if (audioTabButton != null) audioTabButton.onClick.AddListener(() => SwitchTab(Tab.Audio));
        if (controlsTabButton != null) controlsTabButton.onClick.AddListener(() => SwitchTab(Tab.Controls));
        if (videoTabButton != null) videoTabButton.onClick.AddListener(() => SwitchTab(Tab.Video));

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(v =>
            {
                if (sensitivityValueText != null) sensitivityValueText.text = v.ToString("F1");
            });
        if (fovSlider != null)
            fovSlider.onValueChanged.AddListener(v =>
            {
                if (fovValueText != null) fovValueText.text = v.ToString("F0");
            });
        if (renderScaleSlider != null)
            renderScaleSlider.onValueChanged.AddListener(v =>
            {
                if (renderScaleValueText != null) renderScaleValueText.text = v.ToString("F1");
            });
        if (shadowDistanceSlider != null)
            shadowDistanceSlider.onValueChanged.AddListener(v =>
            {
                if (shadowDistanceValueText != null) shadowDistanceValueText.text = v.ToString("F0");
            });
    }

    private void SwitchTab(Tab tab)
    {
        currentTab = tab;
        if (audioPanel != null) audioPanel.SetActive(tab == Tab.Audio);
        if (controlsPanel != null) controlsPanel.SetActive(tab == Tab.Controls);
        if (videoPanel != null) videoPanel.SetActive(tab == Tab.Video);

        if (tab == Tab.Controls)
            RefreshKeybindUI();
    }

    private void BuildKeybindUI()
    {
        if (keybindContainer == null || keybindRowPrefab == null) return;

        for (int i = keybindContainer.childCount - 1; i >= 0; i--)
            Destroy(keybindContainer.GetChild(i).gameObject);

        for (int i = 0; i < rebindableActions.Length; i++)
        {
            var row = Instantiate(keybindRowPrefab, keybindContainer);
            var texts = row.GetComponentsInChildren<TMP_Text>();
            var btn = row.GetComponentInChildren<Button>();

            if (texts != null && texts.Length >= 1)
                texts[0].text = rebindableNames[i];
            if (texts != null && texts.Length >= 2)
            {
                var label = texts[1];
                label.text = KeybindManager.GetBindingName(rebindableActions[i]);
                int capturedIndex = i;
                if (btn != null)
                    btn.onClick.AddListener(() => StartRebind(capturedIndex, label));
            }
            else if (btn != null)
            {
                int capturedIndex = i;
                btn.onClick.AddListener(() =>
                {
                    var labels = btn.GetComponentsInChildren<TMP_Text>();
                    if (labels.Length >= 2)
                        StartRebind(capturedIndex, labels[1]);
                });
            }
        }
    }

    private void RefreshKeybindUI()
    {
        if (keybindContainer == null) return;
        for (int i = 0; i < keybindContainer.childCount && i < rebindableActions.Length; i++)
        {
            var texts = keybindContainer.GetChild(i).GetComponentsInChildren<TMP_Text>();
            if (texts != null && texts.Length >= 2)
                texts[1].text = KeybindManager.GetBindingName(rebindableActions[i]);
        }
    }

    private void StartRebind(int index, TMP_Text label)
    {
        rebindingAction = true;
        rebindingWhat = rebindableActions[index];
        rebindLabel = label;
        rebindTimeout = 5f;
        if (label != null) label.text = "Press a key...";
    }

    private void CancelRebind()
    {
        rebindingAction = false;
        if (rebindLabel != null)
            rebindLabel.text = KeybindManager.GetBindingName(rebindingWhat);
        rebindLabel = null;
    }

    private void LoadSettings()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = SettingsManager.masterVolume;
        if (musicVolumeSlider != null) musicVolumeSlider.value = SettingsManager.musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = SettingsManager.sfxVolume;
        if (sensitivitySlider != null) sensitivitySlider.value = SettingsManager.mouseSensitivity;
        if (fullscreenToggle != null) fullscreenToggle.isOn = SettingsManager.isFullscreen;
        if (vsyncToggle != null) vsyncToggle.isOn = SettingsManager.vsyncEnabled;
        if (qualityDropdown != null) qualityDropdown.value = SettingsManager.qualityLevel;
        if (resolutionDropdown != null) resolutionDropdown.value = SettingsManager.resolutionIndex;
        if (fovSlider != null) fovSlider.value = SettingsManager.fieldOfView;
        if (renderScaleSlider != null) renderScaleSlider.value = SettingsManager.renderScale;
        if (shadowQualityDropdown != null) shadowQualityDropdown.value = SettingsManager.shadowQuality;
        if (shadowDistanceSlider != null) shadowDistanceSlider.value = SettingsManager.shadowDistance;
        if (antiAliasingDropdown != null) antiAliasingDropdown.value = SettingsManager.antiAliasing;
        if (textureQualityDropdown != null) textureQualityDropdown.value = SettingsManager.textureQuality;

        if (sensitivityValueText != null) sensitivityValueText.text = SettingsManager.mouseSensitivity.ToString("F1");
        if (fovValueText != null) fovValueText.text = SettingsManager.fieldOfView.ToString("F0");
        if (renderScaleValueText != null) renderScaleValueText.text = SettingsManager.renderScale.ToString("F1");
        if (shadowDistanceValueText != null) shadowDistanceValueText.text = SettingsManager.shadowDistance.ToString("F0");
    }

    private void OnApply()
    {
        if (masterVolumeSlider != null) SettingsManager.masterVolume = masterVolumeSlider.value;
        if (musicVolumeSlider != null) SettingsManager.musicVolume = musicVolumeSlider.value;
        if (sfxVolumeSlider != null) SettingsManager.sfxVolume = sfxVolumeSlider.value;
        if (sensitivitySlider != null) SettingsManager.mouseSensitivity = sensitivitySlider.value;
        if (fullscreenToggle != null) SettingsManager.isFullscreen = fullscreenToggle.isOn;
        if (vsyncToggle != null) SettingsManager.vsyncEnabled = vsyncToggle.isOn;
        if (qualityDropdown != null) SettingsManager.qualityLevel = qualityDropdown.value;
        if (resolutionDropdown != null) SettingsManager.resolutionIndex = resolutionDropdown.value;
        if (fovSlider != null) SettingsManager.fieldOfView = fovSlider.value;
        if (renderScaleSlider != null) SettingsManager.renderScale = renderScaleSlider.value;
        if (shadowQualityDropdown != null) SettingsManager.shadowQuality = shadowQualityDropdown.value;
        if (shadowDistanceSlider != null) SettingsManager.shadowDistance = shadowDistanceSlider.value;
        if (antiAliasingDropdown != null) SettingsManager.antiAliasing = antiAliasingDropdown.value;
        if (textureQualityDropdown != null) SettingsManager.textureQuality = textureQualityDropdown.value;

        SettingsManager.ApplyGraphics();
        SettingsManager.ApplyRenderScale();
        SettingsManager.ApplyAudio();
        SettingsManager.Save();
    }

    private void OnReset()
    {
        SettingsManager.ResetDefaults();
        KeybindManager.ResetToDefaults();
        LoadSettings();
        RefreshKeybindUI();
    }

    private void OnBack()
    {
        rebindingAction = false;
        gameObject.SetActive(false);
    }
}
