using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _exitToDesktopBtn;
    [SerializeField] private Button _exitLobbyBtn;
    [SerializeField] private Button _readyButton;
    [SerializeField] private TMP_Text _lobbyList;
    private Dictionary<string, bool> playersReady = new Dictionary<string, bool>(); // playerName to isReady

    private void Awake()
    {
        _readyButton.onClick.AddListener(readyButtonClicked);
    }

    private void readyButtonClicked()
    {
        // TODO Get current players name, somehow
        // UpdateLobbyListServerRpc(NetworkManager.Singleton.)
    }

    void Update()
    {
        if (!IsServer) return;
        string text = "";
        foreach (KeyValuePair<string, bool> player in playersReady) 
        {
            if (player.Value)
            {
                text += player.Key + "- READY\n";
                continue;
            }
            text += player.Key + "- WAITING\n";
        }
        UpdateLobbyListTextClientRpc(text);
    }

    // UpdateConnectedClients updates the LobbyList's local cache of connected playerNames
    [ServerRpc(RequireOwnership = false)]
    public void UpdateLobbyListServerRpc(string playerName, bool ready)
    {
        playersReady[playerName] = ready;
    }

    [ClientRpc]
    private void UpdateLobbyListTextClientRpc(string text)
    {
        _lobbyList.text = text;
    }
}
