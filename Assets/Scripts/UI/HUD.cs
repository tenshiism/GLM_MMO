using UnityEngine;

public class HUD : MonoBehaviour
{
    public Color hpBarColor = Color.red;
    public Color xpBarColor = Color.cyan;
    public Color ammoColor = Color.white;

    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private CharacterLevel charLevel;
    private WeaponManager weaponManager;

    private GUIStyle questHeaderStyle;
    private GUIStyle questTextStyle;
    private GUIStyle questProgressStyle;
    private GUIStyle hudBoxStyle;
    private GUIStyle hudLabelStyle;
    private bool stylesInitialized;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        charLevel = GetComponent<CharacterLevel>();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void OnGUI()
    {
        InitStyles();
        DrawHealthBar();
        DrawXPBar();
        DrawAmmo();
        DrawWeaponSlot();
        DrawInteractionPrompt();
        DrawQuestTracker();
        DrawExtractionTimer();
    }

    private void InitStyles()
    {
        if (stylesInitialized) return;
        stylesInitialized = true;

        hudBoxStyle = new GUIStyle(GUI.skin.box);
        hudBoxStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.6f));

        hudLabelStyle = new GUIStyle(GUI.skin.label);
        hudLabelStyle.normal.textColor = Color.white;
        hudLabelStyle.fontStyle = FontStyle.Bold;

        questHeaderStyle = new GUIStyle(GUI.skin.label);
        questHeaderStyle.normal.textColor = new Color(1f, 0.85f, 0.4f);
        questHeaderStyle.fontStyle = FontStyle.Bold;
        questHeaderStyle.fontSize = 14;

        questTextStyle = new GUIStyle(GUI.skin.label);
        questTextStyle.normal.textColor = Color.white;
        questTextStyle.fontSize = 12;

        questProgressStyle = new GUIStyle(GUI.skin.label);
        questProgressStyle.normal.textColor = Color.cyan;
        questProgressStyle.fontSize = 12;
        questProgressStyle.fontStyle = FontStyle.Bold;
    }

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        var tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private void DrawHealthBar()
    {
        float hp = playerHealth != null ? playerHealth.CurrentHealth.Value : 100f;
        float maxHp = playerStats != null ? playerStats.MaxHealth : 100f;

        GUI.Label(new Rect(20, 20, 300, 30), $"HP: {hp:F0}/{maxHp:F0}");

        GUI.Box(new Rect(20, 50, 250, 20), "");
        float hpPct = Mathf.Clamp01(hp / maxHp);
        GUI.color = hpBarColor;
        GUI.Box(new Rect(20, 50, 250 * hpPct, 20), "");
        GUI.color = Color.white;
    }

    private void DrawXPBar()
    {
        int xp = charLevel != null ? charLevel.CurrentXP.Value : 0;
        int xpMax = charLevel != null ? charLevel.XPToNextLevel.Value : 100;
        int level = charLevel != null ? charLevel.Level.Value : 1;

        GUI.Label(new Rect(20, 80, 300, 30), $"Lv.{level} XP: {xp}/{xpMax}");

        GUI.Box(new Rect(20, 110, 250, 15), "");
        float xpPct = xpMax > 0 ? Mathf.Clamp01((float)xp / xpMax) : 0f;
        GUI.color = xpBarColor;
        GUI.Box(new Rect(20, 110, 250 * xpPct, 15), "");
        GUI.color = Color.white;
    }

    private void DrawAmmo()
    {
        if (weaponManager == null || weaponManager.ActiveWeapon == null) return;

        var weapon = weaponManager.ActiveWeapon;
        int ammo = weapon.currentAmmo;
        int maxAmmo = weapon.MaxAmmo;
        bool reloading = weapon.IsReloading;

        float x = Screen.width - 200;
        float y = Screen.height - 60;

        string text = reloading
            ? $"RELOADING... {weapon.ReloadProgress * 100f:F0}%"
            : $"Ammo: {ammo}/{maxAmmo}";

        GUI.color = ammo <= 0 ? Color.red : ammoColor;
        GUI.Label(new Rect(x, y, 180, 30), text);
        GUI.color = Color.white;
    }

    private void DrawWeaponSlot()
    {
        if (weaponManager == null) return;

        float x = Screen.width - 200;
        float y = Screen.height - 90;

        for (int i = 0; i < 3; i++)
        {
            Color c = i == weaponManager.ActiveSlot ? Color.yellow : new Color(0.5f, 0.5f, 0.5f);
            GUI.color = c;
            string label = $"[{i + 1}]";
            GUI.Label(new Rect(x + i * 40, y, 40, 20), label);
        }
        GUI.color = Color.white;

        string weaponName = weaponManager.ActiveWeapon?.definition?.weaponName ?? "";
        if (!string.IsNullOrEmpty(weaponName))
            GUI.Label(new Rect(x, y + 18, 180, 20), weaponName);
    }

    private void DrawInteractionPrompt()
    {
        Interactable[] all = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var i in all)
        {
            if (i != null && i.playerInRange && !string.IsNullOrEmpty(i.interactPrompt))
            {
                float cx = Screen.width / 2f;
                float cy = Screen.height / 2f + 60;
                string prompt = $"[E] {i.interactPrompt}";
                float pw = GUI.skin.label.CalcSize(new GUIContent(prompt)).x;
                GUI.color = new Color(1, 1, 0.8f);
                GUI.Label(new Rect(cx - pw / 2, cy, pw + 20, 30), prompt);
                GUI.color = Color.white;
                return;
            }
        }
    }

    private void DrawQuestTracker()
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.HasQuest) return;

        var q = QuestManager.Instance.ActiveQuest;
        float panelW = 280;
        float panelH = 100;
        float panelX = Screen.width - panelW - 15;
        float panelY = 15;

        GUI.Box(new Rect(panelX, panelY, panelW, panelH), "");

        GUI.Label(new Rect(panelX + 10, panelY + 5, panelW - 20, 22), q.questName, questHeaderStyle);

        string typeStr = q.questType.ToString();
        GUI.Label(new Rect(panelX + 10, panelY + 28, panelW - 20, 18),
            $"<color=#AAAAAA>{typeStr}</color>", questTextStyle);

        if (q.killTarget > 0)
        {
            int kills = QuestManager.Instance.killCount;
            float pct = q.killTarget > 0 ? (float)kills / q.killTarget : 0f;
            string barText = $"Kills: {kills}/{q.killTarget}";
            GUI.Label(new Rect(panelX + 10, panelY + 48, panelW - 20, 18), barText, questTextStyle);

            GUI.Box(new Rect(panelX + 10, panelY + 68, panelW - 20, 10), "");
            GUI.color = new Color(0.2f, 0.8f, 0.2f);
            GUI.Box(new Rect(panelX + 10, panelY + 68, (panelW - 20) * Mathf.Clamp01(pct), 10), "");
            GUI.color = Color.white;
        }
        else if (q.questType == QuestType.Boss)
        {
            GUI.Label(new Rect(panelX + 10, panelY + 48, panelW - 20, 18),
                "Defeat the boss", questTextStyle);
        }
        else if (q.questType == QuestType.Survival)
        {
            GUI.Label(new Rect(panelX + 10, panelY + 48, panelW - 20, 18),
                "Survive the waves", questTextStyle);
        }
        else if (q.questType == QuestType.Collection)
        {
            GUI.Label(new Rect(panelX + 10, panelY + 48, panelW - 20, 18),
                $"Collect {q.collectCount}x {q.collectItemId}", questTextStyle);
        }
    }

    private void DrawExtractionTimer()
    {
        var extractor = FindFirstObjectByType<ExtractionZone>();
        if (extractor == null) return;

        if (!extractor.isActiveAndEnabled) return;

        float cx = Screen.width / 2f;
        float y = Screen.height - 120;
        string text = "EXTRACTING...";
        float pw = GUI.skin.label.CalcSize(new GUIContent(text)).x;

        GUI.color = Color.yellow;
        GUI.Label(new Rect(cx - pw / 2, y, pw + 20, 30), text);
        GUI.color = Color.white;
    }
}
