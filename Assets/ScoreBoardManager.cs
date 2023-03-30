using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreBoardManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _scoreBoardText;
    private Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>(); // playerName to PlayerManager

    void Update()
    {
        if (!IsServer) return;
        string text = "";
        foreach (KeyValuePair<string, PlayerManager> player in players) 
        {
            // TODO Sort by score
            text += player.Key + ": " + player.Value._health.Value + "\n";
        }
        UpdateScoreBoardTextClientRpc(text);
    }

    // UpdateConnectedClients updates the ScoreBoardManagers local cache of connected playerNames
    [ServerRpc(RequireOwnership = false)]
    public void UpdateConnectedClientsServerRpc(ulong clientId, string playerName)
    {
        PlayerManager newPlayer = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerManager>();
        Debug.Log(playerName);
        players[playerName] = newPlayer;
    }

    [ClientRpc]
    private void UpdateScoreBoardTextClientRpc(string text)
    {
        _scoreBoardText.text = text;
    }
}
