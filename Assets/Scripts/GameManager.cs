using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// GameManager manages the overall state of the game
public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartGame(); // TODO Run once all players have loaded the scene (and maybe other conditions?)
    }

    private void StartGame()
    {
        // Create a PlayerObject (_playerPrefab) for each connected client
        foreach (var clients in NetworkManager.Singleton.ConnectedClients)
        {
            SpawnPlayerObject(clients.Key);
        }
    }

    // TODO EndGame()

    private void SpawnPlayerObject(ulong clientId)
    {
        var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        var newPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
        newPlayer.SpawnWithOwnership(clientId, true);
    }

    // TODO DespawnPlayerObject...
}
