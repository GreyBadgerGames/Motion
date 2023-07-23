using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// GameManager manages the overall state of the game
public class GameManager : NetworkBehaviour
{
    public GameMode _game;
    
    public void Start()
    {
        // Hack to allow starting the game in editor from Game scene :)
        if (NetworkManager.Singleton == null) SceneManager.LoadScene("Startup");
    }

    public void FixedUpdate()
    {
        _game.CheckGameState();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"Making a call to Start the GameMode");

        _game = GameObject.Find("GameMode").GetComponent<GameMode>();
        if (_game == null) throw new System.Exception("Could not find 'GameMode' object - was it created in the lobby?");
    
        if (!IsServer) return;
        _game.StartGame(); // TODO Run once all players have loaded the scene (and maybe other conditions?)
    }

    [ServerRpc]
    public void ReportDeathServerRpc(ulong killedClientId, ulong killerClientId)
    {
        Debug.Log($"{killedClientId} killed by {killerClientId}!!");
        _game.PlayerKilled(killedClientId, killerClientId);
    }
}
