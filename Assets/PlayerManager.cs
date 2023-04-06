using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] public GamePlayerManager GamePlayerManager;
    [SerializeField] private GameObject _camera;
    private string _name = "";

    public override void OnNetworkSpawn()
    {
        GamePlayerManager.gameObject.SetActive(false);
        _camera.gameObject.SetActive(false);
    }

    // private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    // {    
    //     Debug.Log("YOOO CHANGED SCENE!");  
    //     switch (sceneEvent.SceneEventType) 
    //     {
    //         case SceneEventType.LoadComplete:
    //             {
    //                 if (sceneEvent.SceneName == "Game") {
    //                     SetActiveGamePlayerObject(true);
    //                     break;
    //                 }
    //                 SetActiveGamePlayerObject(false);
    //                 break;
    //             }
    //     }
    // }

    void Start() 
    {
        if (!IsOwner) return;

        // Get the scene
        // Update current context
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

    [ClientRpc]
    public void SetActiveGamePlayerObjectClientRpc(bool active)
    {
        GamePlayerManager.gameObject.SetActive(active);
        _camera.gameObject.SetActive(active);
    }
}
