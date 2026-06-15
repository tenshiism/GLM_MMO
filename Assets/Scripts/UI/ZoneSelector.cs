using UnityEngine;

public class ZoneSelector : Interactable
{
    [System.Serializable]
    public class ZoneEntry
    {
        public string zoneName;
        public string sceneName;
        public string description;
        public int minLevel;
    }

    [Header("Zones")]
    public ZoneEntry[] zones = new ZoneEntry[]
    {
        new ZoneEntry { zoneName = "Jungle", sceneName = "SampleScene", description = "Dense jungle with raptors and bats", minLevel = 1 },
    };

    private bool showUI;
    private int selectedZone;
    private Vector2 scrollPos;

    private GUIStyle headerStyle;
    private GUIStyle descStyle;
    private GUIStyle zoneNameStyle;
    private GUIStyle lockedStyle;
    private GUIStyle buttonStyle;
    private bool stylesInit;

    private void Start()
    {
        interactPrompt = "Zone Portal (E)";
    }

    public override void OnInteract(GameObject player)
    {
        showUI = !showUI;
        if (showUI)
        {
            UIBlocker.Open();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            UIBlocker.Close();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void InitStyles()
    {
        if (stylesInit) return;
        stylesInit = true;

        headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = new Color(1f, 0.85f, 0.4f);
        headerStyle.alignment = TextAnchor.UpperCenter;

        descStyle = new GUIStyle(GUI.skin.label);
        descStyle.fontSize = 11;
        descStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        descStyle.wordWrap = true;

        zoneNameStyle = new GUIStyle(GUI.skin.label);
        zoneNameStyle.fontSize = 14;
        zoneNameStyle.fontStyle = FontStyle.Bold;
        zoneNameStyle.normal.textColor = Color.white;

        lockedStyle = new GUIStyle(GUI.skin.label);
        lockedStyle.fontSize = 11;
        lockedStyle.normal.textColor = Color.red;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 13;
    }

    private void OnGUI()
    {
        if (!showUI) return;
        InitStyles();

        float w = 450;
        float h = 350;
        float x = (Screen.width - w) / 2;
        float y = (Screen.height - h) / 2;

        GUI.Box(new Rect(x, y, w, h), "");
        GUI.Label(new Rect(x, y + 8, w, 30), "SELECT HUNTING GROUND", headerStyle);

        var playerLevel = 1;
        var cl = FindFirstObjectByType<CharacterLevel>();
        if (cl != null) playerLevel = cl.Level.Value;

        float contentY = y + 45;
        float contentH = h - 100;

        GUI.BeginGroup(new Rect(x + 10, contentY, w - 20, contentH));

        scrollPos = GUI.BeginScrollView(new Rect(0, 0, w - 20, contentH), scrollPos,
            new Rect(0, 0, w - 40, zones.Length * 75));

        for (int i = 0; i < zones.Length; i++)
        {
            var zone = zones[i];
            float zy = i * 75;
            bool canAccess = playerLevel >= zone.minLevel;

            Color bgColor = i == selectedZone
                ? new Color(0.3f, 0.5f, 0.3f, 0.8f)
                : new Color(0.15f, 0.15f, 0.15f, 0.8f);

            GUI.color = bgColor;
            GUI.Box(new Rect(0, zy, w - 40, 70), "");
            GUI.color = Color.white;

            GUI.Label(new Rect(10, zy + 5, 250, 22), zone.zoneName, zoneNameStyle);
            GUI.Label(new Rect(10, zy + 28, w - 60, 30), zone.description, descStyle);

            if (!canAccess)
            {
                GUI.Label(new Rect(10, zy + 50, 200, 18), $"Requires Level {zone.minLevel}", lockedStyle);
            }

            if (GUI.Button(new Rect(w - 130, zy + 15, 80, 30), canAccess ? "Warp" : "Locked"))
            {
                if (canAccess)
                {
                    selectedZone = i;
                    WarpToZone(zone);
                }
            }
        }

        GUI.EndScrollView();

        if (GUI.Button(new Rect(0, contentH - 35, 100, 30), "Close"))
            CloseUI();

        GUI.EndGroup();
    }

    private void WarpToZone(ZoneEntry zone)
    {
        if (!string.IsNullOrEmpty(zone.sceneName))
        {
            CloseUI();
            SceneLoader.LoadScene(zone.sceneName);
        }
    }

    private void CloseUI()
    {
        showUI = false;
        UIBlocker.Close();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (showUI)
        {
            UIBlocker.Close();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
