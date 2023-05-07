using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class GameMode : NetworkBehaviour
{
    [SerializeField] protected NetworkObject _playerPrefab;
    [SerializeField] protected List<Vector3> _spawnLocations;
    protected System.DateTime _gameStartTime;
    public abstract string _name { get; }
    public abstract string _decription { get; }
    public abstract void StartGame();
    public abstract void CheckGameState();
    public abstract void EndGame();
    public abstract void PlayerKilled(ulong killedClientId, ulong killerClientId);
    public abstract string GameStatusToString();
    
    public override void OnNetworkSpawn()
    {
        this.name = "GameMode"; // Set name to GameMode so everyone can find it
    }

    protected void spawnPlayerObject(ulong clientId, Vector3 spawnLoc, NetworkObject prefab)
    {
        Debug.Log($"Spawn PlayerObject for {clientId} at spawn location: {spawnLoc}");
        var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        var newPlayer = Instantiate(prefab, spawnLoc, Quaternion.LookRotation(Vector3.zero));
        newPlayer.SpawnWithOwnership(clientId, true);
        playerNetworkObject.GetComponent<PersistentPlayerManager>().gamePlayer = newPlayer.GetComponentInChildren<GamePlayerManager>();
    }

    protected void respawnPlayer(ulong clientId, Vector3 spawnLoc)
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

    protected void despawnPlayer(ulong client)
    {
        // TODO
        throw new System.NotImplementedException();
    }

    protected int gameTimeElapsed()
    {
        return System.DateTime.Now.Subtract(_gameStartTime).Seconds;
    }
}
