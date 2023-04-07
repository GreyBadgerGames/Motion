using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PersistentPlayerManager : NetworkBehaviour
{
    private string _name = "";

    public override void OnNetworkSpawn()
    {
        Debug.Log($"PersistentPlayer spawned - clientId {NetworkManager.LocalClientId}");
        base.OnNetworkSpawn();
    }

    public void SetName(string name)
    {
        _name = name;
        gameObject.name = "PersistentPlayer-" + _name;
        Debug.Log($"Renamed PersistentPlayer to {_name}");
    }

    private void CreateScoreboard()
    {
        // TODO Improve this, hack just to get the name in
        NetworkManagerUI networkManagerUI = GameObject.Find("NetworkManagerUI").GetComponent<NetworkManagerUI>();
        Debug.Log("NAME FIELD "+ networkManagerUI.nameField.text);
        _name = networkManagerUI.nameField.text;
        if (networkManagerUI == null || networkManagerUI.nameField == null || networkManagerUI.nameField.text == "")
        {
            // Override with clientId
            _name= NetworkManager.Singleton.LocalClientId.ToString();
            Debug.Log("NAME FIELD OVERRIDE " + NetworkManager.Singleton.LocalClientId.ToString());
        }
        // Hack end

        // TODO On disconnect, remove from scoreboard
        ScoreBoardManager scoreBoardUI = GameObject.Find("ScoreBoardUI").GetComponent<ScoreBoardManager>();
        if (scoreBoardUI != null)
        {
            scoreBoardUI.UpdateConnectedClientsServerRpc(NetworkManager.LocalClientId, _name);
        }
    }
}
