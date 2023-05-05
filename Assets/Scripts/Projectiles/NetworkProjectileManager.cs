using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkProjectileManager : GenericProjectileManager
{
    private ulong _clientId;
    private NetworkObject _networkObject;
   
    public override void Update()
    {
        if (!IsServer) return;

        SetColliderRotation();
        SetColliderLength();
    }
    
    // Spawn creates the projectile on the network
    public override void Spawn(ulong clientId)
    {
        _networkObject = gameObject.GetComponent<NetworkObject>();
        _clientId = clientId;
        _networkObject.CheckObjectVisibility = ((clientIdToCheck) =>
        {
            return !(!IsServer && clientIdToCheck == _clientId);
        });

        _networkObject.Spawn();
    }

    public override void OnCollisionEnter(Collision collision) 
    {
        if (!IsServer || _hasCollided) return;
        _hasCollided = true;
        
        _networkObject.Despawn();    
    } 
}
