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

    private void CreateScoreboard()
    {
        // TODO Improve this, hack just to get the name in
        // NetworkManagerUI networkManagerUI = GameObject.Find("NetworkManagerUI").GetComponent<NetworkManagerUI>();
        // Debug.Log("NAME FIELD "+ networkManagerUI.nameField.text);
        // _name.Value = networkManagerUI.nameField.text;
        // if (networkManagerUI == null || networkManagerUI.nameField == null || networkManagerUI.nameField.text == "")
        // {
        //     // Override with clientId
        //     _name.Value = NetworkManager.Singleton.LocalClientId.ToString();
        //     Debug.Log("NAME FIELD OVERRIDE " + NetworkManager.Singleton.LocalClientId.ToString());
        // }
        // // Hack end

        // // TODO On disconnect, remove from scoreboard
        // ScoreBoardManager scoreBoardUI = GameObject.Find("ScoreBoardUI").GetComponent<ScoreBoardManager>();
        // if (scoreBoardUI != null)
        // {
        //     scoreBoardUI.UpdateConnectedClientsServerRpc(NetworkManager.LocalClientId, _name.Value);
        // }
    }
}
