using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// GameManager manages the overall state of the game
public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private List<Vector3> m_spawnLocations;
    [SerializeField] public int m_maxRounds = 2;
    public NetworkVariable<int> m_numOfRounds = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
    
        startGame(); // TODO Run once all players have loaded the scene (and maybe other conditions?)
    }

    private void startGame()
    {
        int i = 0;
        // Create a PlayerObject (_playerPrefab) for each connected client
        foreach (var clients in NetworkManager.Singleton.ConnectedClients)
        {
            spawnPlayerObject(clients.Key, m_spawnLocations[i]);            
            i++;
        }
        m_numOfRounds.Value = 1;
    }

    // TODO randomise spawn locations
    private void restartRound()
    {
        int i = 0;
        foreach (var clients in NetworkManager.Singleton.ConnectedClients)
        {
            respawnPlayer(clients.Key, m_spawnLocations[i]);            
            i++;
        }
    }

    [ServerRpc]
    public void ReportDeathServerRpc(ulong killedClientId, ulong killerClientId)
    {
        Debug.Log($"{killedClientId} killed by {killerClientId}!");

        if (m_numOfRounds.Value >= m_maxRounds)
        {
            Debug.Log($"Game Over!");
            // TODO Move to "end scene", and cleanup game objects
            // This below does not work as the gameObjects are unexpectedly killed, 
            // but leaving in to show the game finished (and everyone still connected)
            NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }

        restartRound();

        m_numOfRounds.Value ++;
    }

    private void spawnPlayerObject(ulong clientId, Vector3 spawnLoc)
    {
        var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        var newPlayer = Instantiate(_playerPrefab, spawnLoc, Quaternion.LookRotation(Vector3.zero));
        newPlayer.SpawnWithOwnership(clientId, true);
        playerNetworkObject.GetComponent<PersistentPlayerManager>().gamePlayer = newPlayer.GetComponentInChildren<GamePlayerManager>();
    }

    // TODO DespawnPlayerObject...

    private void respawnPlayer(ulong clientId, Vector3 spawnLoc)
    {
        var gamePlayer = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<PersistentPlayerManager>().gamePlayer;
        gamePlayer._health.Value = gamePlayer._maxHealth;
        // Set spawn location, looking to the origin
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientId}
            }
        };
        gamePlayer.SetPositionAndRotationClientRpc(spawnLoc, Quaternion.LookRotation(Vector3.zero), clientRpcParams);
    }
}
