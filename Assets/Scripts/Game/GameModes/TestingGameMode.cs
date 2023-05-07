using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class TestingGameMode : GameMode
{
    public override string _name { get{return "Testing - Infinite Respawns";} }

    public override string _decription { get{return "Game mode for testing, infinite respawns enabled.";} }

    public NetworkVariable<int> _numOfRounds = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void StartGame()
    {
        if (!IsServer) return;
        Debug.Log($"Starting Testing GameMode");
        
        int i = 0;
        // Create a PlayerObject (_playerPrefab) for each connected client
        foreach (var clients in NetworkManager.Singleton.ConnectedClients)
        {
            spawnPlayerObject(clients.Key, _spawnLocations[i], _playerPrefab);            
            i++;
        }

        _gameStartTime = System.DateTime.Now;
    }

    public override void CheckGameState()
    {
        // Debug.Log($"Game has been running for {gameTimeElapsed()}");
    }

    public override void EndGame()
    {
        if (!IsServer) return;
        Debug.Log($"Ending game");
        
        // TODO Move to EndGame scene
        this.GetComponent<NetworkObject>().Despawn();
        NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public override void PlayerKilled(ulong killedClientId, ulong killerClientId)
    {
        restartRound();

        _numOfRounds.Value ++;
    }

    // TODO randomise spawn locations
    private void restartRound()
    {
        int i = 0;
        foreach (var clients in NetworkManager.Singleton.ConnectedClients)
        {
            respawnPlayer(clients.Key, _spawnLocations[i]);            
            i++;
        }
    }

    public override string GameStatusToString()
    {
        return $"Round {_numOfRounds.Value}";
    }
}
