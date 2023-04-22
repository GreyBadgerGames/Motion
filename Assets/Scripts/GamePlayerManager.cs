using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// GamePlayerManager runs the player specific logic when in game
public class GamePlayerManager : NetworkBehaviour
{
    [SerializeField] public float _maxHealth = 100;
    public NetworkVariable<float> _health = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private GameManager m_gameManager;

    public override void OnNetworkSpawn()
    {
        setPlayerHealth();
        assignToPersistentPlayer();
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
    public void ReportDamageServerRpc(float mod, ulong damagerId)
    {
        // TODO Check this report can be legitimate
        _health.Value += mod;
        if (_health.Value > _maxHealth)
        {
            _health.Value = _maxHealth;
        }
        else if (_health.Value <= 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().ReportDeathServerRpc(OwnerClientId, damagerId);
        }
    }

    // assignToPersistentPlayer runs on client to assign this to the owning PersistentPlayerObject
    private void assignToPersistentPlayer()
    {
        if (!IsClient || !IsOwner) return;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>().gamePlayer = this;
    }

    [ClientRpc]
    public void SetPositionAndRotationClientRpc(Vector3 pos, Quaternion rotation, ClientRpcParams clientRpcParams = default)
    {
        transform.SetPositionAndRotation(pos, rotation);
    }
}
