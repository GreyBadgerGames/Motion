using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _exitToDesktopBtn;
    [SerializeField] private Button _exitLobbyBtn;
    [SerializeField] private Button _readyButton;
    [SerializeField] private TMP_Text _lobbyList;
    private bool ready;

    public override void OnNetworkSpawn()
    {
        UpdateLobbyListServerRpc(NetworkManager.LocalClientId, ready);

        _readyButton.onClick.AddListener(readyButtonClicked);
        _exitLobbyBtn.onClick.AddListener(exitLobbyButtonClicked);
        _exitToDesktopBtn.onClick.AddListener(exitToDesktopButtonClicked);
        base.OnNetworkSpawn();
    }

    private void readyButtonClicked()
    {
        ready = !ready;
        UpdateLobbyListServerRpc(NetworkManager.LocalClientId, ready);
        checkAllPlayersReady();
    }

    private void exitLobbyButtonClicked()
    {
        if (IsHost) // TODO Remove - hack for host local testing
        {
            SceneManager.LoadScene("MainMenu");
            NetworkManager.Singleton.Shutdown();
            return;
        }

        Debug.LogWarning($"Requesting to disconnect {NetworkManager.LocalClientId}");
        disconnectClientServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc (RequireOwnership = false)]
    private void disconnectClientServerRpc(ulong clientId)
    {
        Debug.LogWarning($"Disconnecting {clientId}");
        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    private void exitToDesktopButtonClicked()
    {
        Application.Quit();
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        string text = "";
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayerManager player = client.Value.PlayerObject.GetComponent<PersistentPlayerManager>();
            if (player.readyInLobby.Value)
            {
                text += player.GetName() + " - READY\n";
                continue;
            }
            text += player.GetName() + " - WAITING\n";
        }

        updateLobbyListTextClientRpc(text);
    }

    private void checkAllPlayersReady()
    {
        int numReady = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayerManager player = client.Value.PlayerObject.GetComponent<PersistentPlayerManager>();
            if (player.readyInLobby.Value)
            {
                numReady ++;
            }
        }

        if (numReady == NetworkManager.Singleton.ConnectedClients.Count) {
            var status = NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load Game " +
                    $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }

    // UpdateConnectedClients updates the LobbyList's local cache of connected playerNames
    [ServerRpc(RequireOwnership = false)]
    public void UpdateLobbyListServerRpc(ulong clientId, bool ready)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PersistentPlayerManager>().readyInLobby.Value = ready;
    }

    [ClientRpc]
    private void updateLobbyListTextClientRpc(string text)
    {
        _lobbyList.text = text;
    }
}
