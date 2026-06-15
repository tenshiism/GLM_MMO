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

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        charLevel = GetComponent<CharacterLevel>();
        weaponManager = GetComponent<WeaponManager>();
    }

    private void OnGUI()
    {
        DrawHealthBar();
        DrawXPBar();
        DrawAmmo();
        DrawInteractionPrompt();
        DrawQuestStatus();
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

    private void DrawQuestStatus()
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.HasQuest) return;

        var q = QuestManager.Instance.ActiveQuest;
        float x = Screen.width / 2f - 150;
        float y = 20;

        GUI.Box(new Rect(x, y, 300, 60), "");
        GUI.Label(new Rect(x + 10, y + 5, 280, 25), $"<b>Quest: {q.questName}</b>");
        if (q.killTarget > 0)
        {
            int kills = QuestManager.Instance.killCount;
            GUI.Label(new Rect(x + 10, y + 30, 280, 25), $"Kills: {kills}/{q.killTarget}");
        }
    }
}
