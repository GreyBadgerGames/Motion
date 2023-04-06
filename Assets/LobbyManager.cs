using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
        // TODO HACK TEST REMOVE
        if (IsServer && !string.IsNullOrEmpty("Lobby"))
        {
            var status = NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load Game " +
                    $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }

    // void Update()
    // {
    //     if (!IsServer) return;
    //     string text = "";
    //     foreach (KeyValuePair<string, bool> player in playersReady) 
    //     {
    //         if (player.Value)
    //         {
    //             text += player.Key + "- READY\n";
    //             continue;
    //         }
    //         text += player.Key + "- WAITING\n";
    //     }
    //     // UpdateLobbyListTextClientRpc(text); TODO ADD BACK ON
    // }

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
