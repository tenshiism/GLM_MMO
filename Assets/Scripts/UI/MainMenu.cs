using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    private void Start()
    {
        var startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
        var optionsButton = GameObject.Find("OptionsButton")?.GetComponent<Button>();
        var quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = SettingsManager.mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    private void OnStartClicked()
    {
        SceneLoader.LoadGame();
    }

    private void OnOptionsClicked()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
        SceneLoader.QuitGame();
    }

    private void OnSensitivityChanged(float value)
    {
        SettingsManager.mouseSensitivity = value;
    }

    private void OnBackClicked()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
}
