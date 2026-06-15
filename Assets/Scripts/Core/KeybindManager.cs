using UnityEngine;
using System;
using System.Collections.Generic;

public static class KeybindManager
{
    public enum Action
    {
        MoveForward,
        MoveBack,
        StrafeLeft,
        StrafeRight,
        Jump,
        Fire,
        AltFire,
        Reload,
        Interact,
        UseItem,
        Ping,
        Slot1,
        Slot2,
        Slot3,
        NextWeapon,
        PrevWeapon,
        OpenMenu,
        OpenInventory,
        Sprint,
        Crouch
    }

    private static readonly Dictionary<Action, KeyCode> defaults = new Dictionary<Action, KeyCode>
    {
        { Action.MoveForward, KeyCode.W },
        { Action.MoveBack, KeyCode.S },
        { Action.StrafeLeft, KeyCode.A },
        { Action.StrafeRight, KeyCode.D },
        { Action.Jump, KeyCode.Space },
        { Action.Fire, KeyCode.Mouse0 },
        { Action.AltFire, KeyCode.Mouse1 },
        { Action.Reload, KeyCode.R },
        { Action.Interact, KeyCode.E },
        { Action.UseItem, KeyCode.Q },
        { Action.Ping, KeyCode.Mouse2 },
        { Action.Slot1, KeyCode.Alpha1 },
        { Action.Slot2, KeyCode.Alpha2 },
        { Action.Slot3, KeyCode.Alpha3 },
        { Action.NextWeapon, KeyCode.None },
        { Action.PrevWeapon, KeyCode.None },
        { Action.OpenMenu, KeyCode.Escape },
        { Action.OpenInventory, KeyCode.Tab },
        { Action.Sprint, KeyCode.LeftShift },
        { Action.Crouch, KeyCode.LeftControl }
    };

    private static Dictionary<Action, KeyCode> bindings;
    private static bool loaded;

    public static event Action<Action, KeyCode> OnKeyRebound;

    private static void EnsureLoaded()
    {
        if (loaded) return;
        bindings = new Dictionary<Action, KeyCode>();
        foreach (var kvp in defaults)
            bindings[kvp.Key] = kvp.Key == Action.NextWeapon || kvp.Key == Action.PrevWeapon
                ? kvp.Value
                : kvp.Value;
        Load();
        loaded = true;
    }

    public static KeyCode GetBinding(Action action)
    {
        EnsureLoaded();
        return bindings.ContainsKey(action) ? bindings[action] : KeyCode.None;
    }

    public static string GetBindingName(Action action)
    {
        KeyCode key = GetBinding(action);
        return KeyToString(key);
    }

    public static void Rebind(Action action, KeyCode newKey)
    {
        EnsureLoaded();
        bindings[action] = newKey;
        Save();
        OnKeyRebound?.Invoke(action, newKey);
    }

    public static void ResetToDefaults()
    {
        EnsureLoaded();
        bindings = new Dictionary<Action, KeyCode>(defaults);
        Save();
    }

    public static bool IsActionHeld(Action action)
    {
        return UnityEngine.Input.GetKey(GetBinding(action));
    }

    public static bool IsActionDown(Action action)
    {
        return UnityEngine.Input.GetKeyDown(GetBinding(action));
    }

    public static bool IsActionUp(Action action)
    {
        return UnityEngine.Input.GetKeyUp(GetBinding(action));
    }

    public static float GetAxis(string horizontalAction, string verticalAction)
    {
        float h = 0f;
        float v = 0f;
        if (KeyboardInput.GetKey(KeyCode.A)) h -= 1f;
        if (KeyboardInput.GetKey(KeyCode.D)) h += 1f;
        if (KeyboardInput.GetKey(KeyCode.W)) v += 1f;
        if (KeyboardInput.GetKey(KeyCode.S)) v -= 1f;
        return h;
    }

    public static Vector2 GetMoveInput()
    {
        EnsureLoaded();
        Vector2 input = Vector2.zero;
        if (IsActionHeld(Action.StrafeRight)) input.x += 1f;
        if (IsActionHeld(Action.StrafeLeft)) input.x -= 1f;
        if (IsActionHeld(Action.MoveForward)) input.y += 1f;
        if (IsActionHeld(Action.MoveBack)) input.y -= 1f;
        return Vector2.ClampMagnitude(input, 1f);
    }

    public static Vector2 GetMouseDelta()
    {
        return new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
    }

    public static float GetScrollDelta()
    {
        return UnityEngine.Input.GetAxis("Mouse ScrollWheel");
    }

    private static void Save()
    {
        foreach (var kvp in bindings)
            PlayerPrefs.SetString("Keybind_" + kvp.Key.ToString(), kvp.Key.ToString());
        PlayerPrefs.Save();
    }

    private static void Load()
    {
        foreach (Action action in Enum.GetValues(typeof(Action)))
        {
            string key = "Keybind_" + action.ToString();
            string saved = PlayerPrefs.GetString(key, "");
            if (!string.IsNullOrEmpty(saved) && Enum.TryParse<KeyCode>(saved, out KeyCode parsed))
                bindings[action] = parsed;
        }
    }

    public static string KeyToString(KeyCode key)
    {
        return key switch
        {
            KeyCode.Space => "Space",
            KeyCode.LeftShift => "LShift",
            KeyCode.RightShift => "RShift",
            KeyCode.LeftControl => "LCtrl",
            KeyCode.RightControl => "RCtrl",
            KeyCode.LeftAlt => "LAlt",
            KeyCode.RightAlt => "RAlt",
            KeyCode.Mouse0 => "LMB",
            KeyCode.Mouse1 => "RMB",
            KeyCode.Mouse2 => "MMB",
            KeyCode.Alpha0 => "0",
            KeyCode.Alpha1 => "1",
            KeyCode.Alpha2 => "2",
            KeyCode.Alpha3 => "3",
            KeyCode.Alpha4 => "4",
            KeyCode.Alpha5 => "5",
            KeyCode.Alpha6 => "6",
            KeyCode.Alpha7 => "7",
            KeyCode.Alpha8 => "8",
            KeyCode.Alpha9 => "9",
            KeyCode.None => "None",
            _ => key.ToString()
        };
    }

    public static bool IsConflict(Action action, KeyCode key)
    {
        EnsureLoaded();
        foreach (var kvp in bindings)
        {
            if (kvp.Key != action && kvp.Value == key && key != KeyCode.None)
                return true;
        }
        return false;
    }
}

public static class KeyboardInput
{
    public static bool GetKey(KeyCode key)
    {
        if (key == KeyCode.None) return false;
        if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
        {
            int btn = (int)key - (int)KeyCode.Mouse0;
            return UnityEngine.Input.GetMouseButton(btn);
        }
        return UnityEngine.Input.GetKey(key);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (key == KeyCode.None) return false;
        if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
        {
            int btn = (int)key - (int)KeyCode.Mouse0;
            return UnityEngine.Input.GetMouseButtonDown(btn);
        }
        return UnityEngine.Input.GetKeyDown(key);
    }

    public static bool GetKeyUp(KeyCode key)
    {
        if (key == KeyCode.None) return false;
        if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
        {
            int btn = (int)key - (int)KeyCode.Mouse0;
            return UnityEngine.Input.GetMouseButtonUp(btn);
        }
        return UnityEngine.Input.GetKeyUp(key);
    }
}
