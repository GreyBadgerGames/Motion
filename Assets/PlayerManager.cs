using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    private NetworkVariable<float> _health = new NetworkVariable<float>();

    void Start() 
    {
        if (!IsServer) return;
        _health.Value = _maxHealth;
    }
}
