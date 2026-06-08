using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        SceneLoader.LoadGame();
    }

    private void OnOptionsClicked()
    {

    }

    private void OnQuitClicked()
    {
        SceneLoader.QuitGame();
    }
}
