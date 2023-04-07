using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// GamePlayerManager runs the player specific logic when in game
public class GamePlayerManager : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    public NetworkVariable<float> _health = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        setPlayerHealth();
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
