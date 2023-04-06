using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
        foreach (KeyValuePair<ulong, NetworkClient> player in NetworkManager.Singleton.ConnectedClients) {
            player.Value.PlayerObject.GetComponent<PlayerManager>().SetActiveGamePlayerObjectClientRpc(true);
        }
    }
}
