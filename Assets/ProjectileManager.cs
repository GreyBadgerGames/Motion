using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Maybe move Projectile to Pooled Dynamic Spawning if we see performance issues?
// https://docs-multiplayer.unity3d.com/netcode/current/basics/object-spawning/index.html#pooled-dynamic-spawning

// ProjectileManager manages a generic projectile, with projectiles stats being set by the firer
public class ProjectileManager : NetworkBehaviour
{
    private float _speed;
    private float _damage;
    private NetworkObject _networkObject;

    // SetupAndSpawn sets up the projectile, and creates it on the network
    public void SetupAndSpawn(float speed, float damage)
    {
        _speed = speed;
        _damage = damage;
        _networkObject = gameObject.GetComponent<NetworkObject>();
        
        gameObject.GetComponent<Rigidbody>().isKinematic = false;

        _networkObject.Spawn();
    }

    // Fire adds an impulse force to the projectile's RigidBidy as an impluse
    public void Fire(Vector3 direction)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(direction * _speed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (!IsOwner) return;
        Debug.Log("Projectile collided with " + collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log(collision.gameObject.GetComponent<NetworkBehaviour>());
            // #5 Call a ServerRPC on Player to damage them
        }
        
        DespawnServerRpc();        
    }

    [ServerRpc]
    private void DespawnServerRpc()
    {
        _networkObject.Despawn();
    }
}
