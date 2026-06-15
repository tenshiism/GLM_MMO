using UnityEngine;
using Unity.Netcode;

public class SessionManager : NetworkBehaviour
{
    public static SessionManager Instance { get; private set; }

    [Header("Session State")]
    public NetworkVariable<bool> questActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> hostClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            hostClientId.Value = NetworkManager.Singleton.LocalClientId;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[Session] Client {clientId} connected");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[Session] Client {clientId} disconnected");
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count <= 1)
        {
            Debug.Log("[Session] All players left, shutting down");
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void StartQuestServerRpc()
    {
        questActive.Value = true;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void CompleteQuestServerRpc()
    {
        questActive.Value = false;
    }
}
