using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Network")]
    public NetworkManager networkManager;
    public int maxPlayers = 4;

    [Header("UI")]
    public GameObject lobbyPanel;
    public GameObject connectingPanel;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (networkManager == null)
            networkManager = FindFirstObjectByType<NetworkManager>();
    }

    public void HostLobby()
    {
        networkManager.StartHost();
        ShowConnecting(false);
    }

    public void JoinLobby(string hostAddress = "127.0.0.1")
    {
        NetworkManager.Singleton.StartClient();
        ShowConnecting(false);
    }

    public void LeaveLobby()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.Shutdown();
        else if (NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.Shutdown();
    }

    private void ShowConnecting(bool show)
    {
        if (lobbyPanel != null) lobbyPanel.SetActive(!show);
        if (connectingPanel != null) connectingPanel.SetActive(show);
    }
}
