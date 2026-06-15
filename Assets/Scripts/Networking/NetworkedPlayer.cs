using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    [Header("Sync")]
    public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> level = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> xp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private PlayerHealth playerHealth;
    private CharacterLevel characterLevel;
    private PlayerController controller;
    private CharacterController characterController;

    public override void OnNetworkSpawn()
    {
        playerHealth = GetComponent<PlayerHealth>();
        characterLevel = GetComponent<CharacterLevel>();
        controller = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();

        if (!IsOwner)
        {
            if (controller != null) controller.enabled = false;
            if (characterController != null) characterController.enabled = false;
            if (GetComponentInChildren<Camera>() != null)
                GetComponentInChildren<Camera>().enabled = false;
            if (GetComponentInChildren<AudioListener>() != null)
                GetComponentInChildren<AudioListener>().enabled = false;
        }

        if (IsServer)
        {
            health.Value = playerHealth != null ? playerHealth.CurrentHealth.Value : 100f;
        }

        health.OnValueChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        if (playerHealth != null)
            playerHealth.CurrentHealth.Value = newValue;
    }

    private void Update()
    {
        if (!IsServer) return;

        if (playerHealth != null)
            health.Value = playerHealth.CurrentHealth.Value;

        if (characterLevel != null)
        {
            level.Value = characterLevel.Level.Value;
            xp.Value = characterLevel.CurrentXP.Value;
        }
    }
}
