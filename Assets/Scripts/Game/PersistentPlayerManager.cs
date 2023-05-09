using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class PersistentPlayerManager : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> _name = new NetworkVariable<FixedString32Bytes>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> readyInLobby = new NetworkVariable<bool>();
    public GamePlayerManager gamePlayer;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"PersistentPlayer spawned - clientId {NetworkManager.LocalClientId}");
        readyInLobby.Value = false;
        base.OnNetworkSpawn();
    }

    [ServerRpc]
    public void SetNameServerRpc(string name)
    {
        _name.Value = name;
        gameObject.name = "PersistentPlayer-" + _name.Value;
        Debug.Log($"Renamed PersistentPlayer to {_name.Value}");
    }

    public FixedString32Bytes GetName()
    {
        return _name.Value;
    }
}
