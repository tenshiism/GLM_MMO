using UnityEngine;
using UnityEngine.InputSystem;

public class DebugTeleport : MonoBehaviour
{
    [Header("Hold T + press number to teleport. Only works in Editor or with DEVELOP build flag.")]

    [Header("HubZone Locations")]
    public Vector3 hubSpawnPlaza = new Vector3(0f, 0f, 15f);
    public Vector3 hubQuestBoard = new Vector3(0f, 0f, -15f);
    public Vector3 hubArmory = new Vector3(-15f, 0f, -8f);
    public Vector3 hubWardrobe = new Vector3(-15f, 0f, 8f);
    public Vector3 hubMedical = new Vector3(15f, 0f, -8f);
    public Vector3 hubPortal = new Vector3(20f, 0f, 0f);

    [Header("Hunting Ground Locations")]
    public Vector3 hgSpawn = new Vector3(300f, 0f, 300f);
    public Vector3 hgRiver = new Vector3(500f, 0f, 500f);
    public Vector3 hgRidge = new Vector3(200f, 50f, 800f);
    public Vector3 hgCanyon = new Vector3(600f, 0f, 200f);
    public Vector3 hgPlateau = new Vector3(750f, 30f, 750f);
    public Vector3 hgBossArena = new Vector3(500f, 0f, 500f);

    private CharacterController controller;
    private bool showHelp;
    private float helpTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.rightShiftKey.isPressed)
            return;

        if (!Keyboard.current.tKey.wasPressedThisFrame)
            return;

        int slot = -1;

        if (Keyboard.current.numpad0Key.wasPressedThisFrame) slot = 0;
        else if (Keyboard.current.numpad1Key.wasPressedThisFrame) slot = 1;
        else if (Keyboard.current.numpad2Key.wasPressedThisFrame) slot = 2;
        else if (Keyboard.current.numpad3Key.wasPressedThisFrame) slot = 3;
        else if (Keyboard.current.numpad4Key.wasPressedThisFrame) slot = 4;
        else if (Keyboard.current.numpad5Key.wasPressedThisFrame) slot = 5;
        else if (Keyboard.current.numpad6Key.wasPressedThisFrame) slot = 6;
        else if (Keyboard.current.numpad7Key.wasPressedThisFrame) slot = 7;
        else if (Keyboard.current.numpad8Key.wasPressedThisFrame) slot = 8;
        else if (Keyboard.current.numpad9Key.wasPressedThisFrame) slot = 9;
        else
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                showHelp = !showHelp;
                helpTimer = 60f;
                return;
            }
            return;
        }

        helpTimer = 4f;
        showHelp = true;
        Teleport(slot);
    }

    private void Teleport(int slot)
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Vector3 pos;

        if (scene == "HubZone")
        {
            pos = slot switch
            {
                0 => hubSpawnPlaza,
                1 => hubQuestBoard,
                2 => hubArmory,
                3 => hubWardrobe,
                4 => hubMedical,
                5 => hubPortal,
                _ => hubSpawnPlaza
            };
        }
        else
        {
            pos = slot switch
            {
                0 => hgSpawn,
                1 => hgRiver,
                2 => hgRidge,
                3 => hgCanyon,
                4 => hgPlateau,
                5 => hgBossArena,
                _ => hgSpawn
            };
        }

        if (controller != null)
        {
            controller.enabled = false;
            transform.position = pos + Vector3.up * 2f;
            controller.enabled = true;
        }
        else
        {
            transform.position = pos + Vector3.up * 2f;
        }

        Debug.Log($"[DebugTeleport] Warped to slot {slot} in {scene}: {pos}");
    }

    private void OnGUI()
    {
        if (!showHelp) return;

        helpTimer -= Time.unscaledDeltaTime;
        if (helpTimer <= 0f)
        {
            showHelp = false;
            return;
        }

        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        float x = 10f;
        float y = Screen.height - 260f;
        float w = 320f;
        float h = 250f;

        GUI.Box(new Rect(x, y, w, h), "");

        GUIStyle header = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 14 };
        GUI.Label(new Rect(x + 10, y + 5, w - 20, 25), "DEBUG TELEPORT (Shift+T)", header);
        GUI.Label(new Rect(x + 10, y + 28, w - 20, 20), $"Scene: {scene}");

        GUIStyle dim = new GUIStyle(GUI.skin.label) { fontSize = 11 };
        dim.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

        if (scene == "HubZone")
        {
            GUI.Label(new Rect(x + 10, y + 50, w - 20, 18), "Numpad 0: Spawn Plaza", dim);
            GUI.Label(new Rect(x + 10, y + 66, w - 20, 18), "Numpad 1: Quest Board", dim);
            GUI.Label(new Rect(x + 10, y + 82, w - 20, 18), "Numpad 2: Armory", dim);
            GUI.Label(new Rect(x + 10, y + 98, w - 20, 18), "Numpad 3: Wardrobe", dim);
            GUI.Label(new Rect(x + 10, y + 114, w - 20, 18), "Numpad 4: Medical Station", dim);
            GUI.Label(new Rect(x + 10, y + 130, w - 20, 18), "Numpad 5: Portal", dim);
        }
        else
        {
            GUI.Label(new Rect(x + 10, y + 50, w - 20, 18), "Numpad 0: Spawn Point", dim);
            GUI.Label(new Rect(x + 10, y + 66, w - 20, 18), "Numpad 1: River Crossing", dim);
            GUI.Label(new Rect(x + 10, y + 82, w - 20, 18), "Numpad 2: Ridge Line", dim);
            GUI.Label(new Rect(x + 10, y + 98, w - 20, 18), "Numpad 3: Canyon Floor", dim);
            GUI.Label(new Rect(x + 10, y + 114, w - 20, 18), "Numpad 4: Plateau Top", dim);
            GUI.Label(new Rect(x + 10, y + 130, w - 20, 18), "Numpad 5: Boss Arena", dim);
        }

        GUI.Label(new Rect(x + 10, y + 160, w - 20, 18), "H: Toggle this help", dim);
        GUI.Label(new Rect(x + 10, y + 178, w - 20, 18), "Works in both HubZone and hunting grounds", dim);
        GUI.Label(new Rect(x + 10, y + 196, w - 20, 18), $"Auto-hides in {helpTimer:F0}s", dim);
    }
}
