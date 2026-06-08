using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
