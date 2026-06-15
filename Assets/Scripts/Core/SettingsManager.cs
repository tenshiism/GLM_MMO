using UnityEngine;

public static class SettingsManager
{
    // Audio
    public static float masterVolume { get; set; } = 1f;
    public static float musicVolume { get; set; } = 0.8f;
    public static float sfxVolume { get; set; } = 1f;

    // Controls
    public static float mouseSensitivity { get; set; } = 4f;

    // Video
    public static bool isFullscreen { get; set; } = true;
    public static bool vsyncEnabled { get; set; } = true;
    public static int qualityLevel { get; set; } = 2;
    public static int resolutionIndex { get; set; }
    public static float fieldOfView { get; set; } = 70f;
    public static float renderScale { get; set; } = 1f;
    public static int shadowQuality { get; set; } = 2;
    public static float shadowDistance { get; set; } = 50f;
    public static int antiAliasing { get; set; } = 0;
    public static int textureQuality { get; set; } = 0;

    private const string PrefPrefix = "Turok26_";

    static SettingsManager()
    {
        Load();
    }

    public static void ApplyGraphics()
    {
        QualitySettings.SetQualityLevel(qualityLevel);

        var resolutions = Screen.resolutions;
        if (resolutions.Length > 0 && resolutionIndex < resolutions.Length)
        {
            var r = resolutions[resolutionIndex];
            Screen.SetResolution(r.width, r.height, isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }

        QualitySettings.vSyncCount = vsyncEnabled ? 1 : 0;
        QualitySettings.antiAliasing = antiAliasing switch
        {
            1 => 2,
            2 => 4,
            3 => 8,
            _ => 0
        };
        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.shadowResolution = shadowQuality switch
        {
            0 => ShadowResolution.Low,
            1 => ShadowResolution.Medium,
            2 => ShadowResolution.High,
            3 => ShadowResolution.VeryHigh,
            _ => ShadowResolution.High
        };
        QualitySettings.globalTextureMipmapLimit = textureQuality;

        Camera main = Camera.main;
        if (main != null)
            main.fieldOfView = fieldOfView;
    }

    public static void ApplyRenderScale()
    {
        var urpAsset = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        if (urpAsset != null)
        {
            var prop = urpAsset.GetType().GetProperty("renderScale");
            if (prop != null)
                prop.SetValue(urpAsset, renderScale);
        }
    }

    public static void ApplyAudio()
    {
        AudioListener.volume = masterVolume;
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat(PrefPrefix + "MasterVolume", masterVolume);
        PlayerPrefs.SetFloat(PrefPrefix + "MusicVolume", musicVolume);
        PlayerPrefs.SetFloat(PrefPrefix + "SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat(PrefPrefix + "Sensitivity", mouseSensitivity);
        PlayerPrefs.SetInt(PrefPrefix + "Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(PrefPrefix + "VSync", vsyncEnabled ? 1 : 0);
        PlayerPrefs.SetInt(PrefPrefix + "Quality", qualityLevel);
        PlayerPrefs.SetInt(PrefPrefix + "Resolution", resolutionIndex);
        PlayerPrefs.SetFloat(PrefPrefix + "FOV", fieldOfView);
        PlayerPrefs.SetFloat(PrefPrefix + "RenderScale", renderScale);
        PlayerPrefs.SetInt(PrefPrefix + "ShadowQuality", shadowQuality);
        PlayerPrefs.SetFloat(PrefPrefix + "ShadowDistance", shadowDistance);
        PlayerPrefs.SetInt(PrefPrefix + "AntiAliasing", antiAliasing);
        PlayerPrefs.SetInt(PrefPrefix + "TextureQuality", textureQuality);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        masterVolume = PlayerPrefs.GetFloat(PrefPrefix + "MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat(PrefPrefix + "MusicVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat(PrefPrefix + "SFXVolume", 1f);
        mouseSensitivity = PlayerPrefs.GetFloat(PrefPrefix + "Sensitivity", 4f);
        isFullscreen = PlayerPrefs.GetInt(PrefPrefix + "Fullscreen", 1) == 1;
        vsyncEnabled = PlayerPrefs.GetInt(PrefPrefix + "VSync", 1) == 1;
        qualityLevel = PlayerPrefs.GetInt(PrefPrefix + "Quality", 2);
        resolutionIndex = PlayerPrefs.GetInt(PrefPrefix + "Resolution", 0);
        fieldOfView = PlayerPrefs.GetFloat(PrefPrefix + "FOV", 70f);
        renderScale = PlayerPrefs.GetFloat(PrefPrefix + "RenderScale", 1f);
        shadowQuality = PlayerPrefs.GetInt(PrefPrefix + "ShadowQuality", 2);
        shadowDistance = PlayerPrefs.GetFloat(PrefPrefix + "ShadowDistance", 50f);
        antiAliasing = PlayerPrefs.GetInt(PrefPrefix + "AntiAliasing", 0);
        textureQuality = PlayerPrefs.GetInt(PrefPrefix + "TextureQuality", 0);
    }

    public static void ResetDefaults()
    {
        masterVolume = 1f;
        musicVolume = 0.8f;
        sfxVolume = 1f;
        mouseSensitivity = 4f;
        isFullscreen = true;
        vsyncEnabled = true;
        qualityLevel = 2;
        resolutionIndex = 0;
        fieldOfView = 70f;
        renderScale = 1f;
        shadowQuality = 2;
        shadowDistance = 50f;
        antiAliasing = 0;
        textureQuality = 0;
        Save();
    }
}
