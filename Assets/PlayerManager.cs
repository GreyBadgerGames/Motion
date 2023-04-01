using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    public NetworkVariable<float> _health = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private string _name = "";
    

    void Start() 
    {
        setPlayerHealth();
        
        if (!IsOwner) return;     
        
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

    void Update()
    {
        if (!IsServer) return;
        
        if (_health.Value <= 0)
        {
            _health.Value = _maxHealth;
        }
    }

    private void setPlayerHealth()
    {
        if (!IsServer) return;
        _health.Value = _maxHealth;
    }

    [ServerRpc (RequireOwnership = false)]
    public void ModifyHealthServerRpc(float mod)
    {
        _health.Value += mod;
        if (_health.Value > _maxHealth)
        {
            _health.Value = _maxHealth;
        }
    }
}
