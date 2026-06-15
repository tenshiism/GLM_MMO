using UnityEngine;
using Unity.Netcode;

public class CharacterLevel : NetworkBehaviour
{
    public NetworkVariable<int> Level = new NetworkVariable<int>(1);
    public NetworkVariable<int> CurrentXP = new NetworkVariable<int>(0);
    public NetworkVariable<int> XPToNextLevel = new NetworkVariable<int>(100);

    public System.Action<int> OnLevelUp;

    private const float XP_SCALE = 1.5f;
    private const int XP_BASE = 100;

    public void AddXP(int amount)
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && !IsServer) return;

        CurrentXP.Value += amount;
        while (CurrentXP.Value >= XPToNextLevel.Value)
        {
            CurrentXP.Value -= XPToNextLevel.Value;
            Level.Value++;
            XPToNextLevel.Value = Mathf.RoundToInt(XPToNextLevel.Value * XP_SCALE);
            OnLevelUp?.Invoke(Level.Value);
        }
    }

    public float GetProgress()
    {
        return (float)CurrentXP.Value / XPToNextLevel.Value;
    }
}
