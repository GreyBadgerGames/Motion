using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreBoardManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _scoreBoardText;
    private Dictionary<string, GamePlayerManager> players = new Dictionary<string, GamePlayerManager>(); // playerName to PlayerManager

    void Update()
    {
        if (!IsServer) return;
        string text = "";
        foreach (KeyValuePair<string, GamePlayerManager> player in players) 
        {
            // TODO Sort by score
            text += player.Key + ": " + player.Value.GetComponent<GamePlayerManager>()._health.Value + "\n";
        }
        UpdateScoreBoardTextClientRpc(text);
    }

    // UpdateConnectedClients updates the ScoreBoardManagers local cache of connected playerNames
    [ServerRpc(RequireOwnership = false)]
    public void UpdateConnectedClientsServerRpc(ulong clientId, string playerName)
    {
        GamePlayerManager newPlayer = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<GamePlayerManager>();
        Debug.Log(playerName);
        players[playerName] = newPlayer;
    }

    [ClientRpc]
    private void UpdateScoreBoardTextClientRpc(string text)
    {
        _scoreBoardText.text = text;
    }
}
