using UnityEngine;

public class MedicalStation : Interactable
{
    [Header("Healing")]
    public float healAmount = 50f;
    public float healCost = 100;

    private bool showUI;

    private void Start()
    {
        interactPrompt = "Medical Station (E)";
    }

    public override void OnInteract(GameObject player)
    {
        if (playerObject == null) playerObject = player;
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

    private void OnGUI()
    {
        if (!showUI) return;

        float w = 350;
        float h = 200;
        float x = (Screen.width - w) / 2;
        float y = (Screen.height - h) / 2;

        GUI.Box(new Rect(x, y, w, h), "Medical Station");

        var health = playerObject != null ? playerObject.GetComponent<PlayerHealth>() : null;
        var stats = playerObject != null ? playerObject.GetComponent<PlayerStats>() : null;

        float hp = health != null ? health.CurrentHealth.Value : 100;
        float maxHp = stats != null ? stats.MaxHealth : 100;

        GUI.Label(new Rect(x + 20, y + 35, 200, 25), $"HP: {hp:F0}/{maxHp:F0}");

        if (hp < maxHp)
        {
            if (GUI.Button(new Rect(x + 20, y + 70, 140, 30), $"Heal {healAmount} ($0)"))
            {
                health.Heal(healAmount);
            }

            if (GUI.Button(new Rect(x + 180, y + 70, 140, 30), $"Full Heal ($0)"))
            {
                health.Heal(maxHp - hp);
            }
        }
        else
        {
            GUI.Label(new Rect(x + 20, y + 70, 200, 30), "Full health.");
        }

        GUI.Label(new Rect(x + 20, y + 115, 300, 25), "Resupply: (future)");

        if (GUI.Button(new Rect(x + w - 110, y + h - 40, 100, 25), "Close"))
            CloseUI();
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
