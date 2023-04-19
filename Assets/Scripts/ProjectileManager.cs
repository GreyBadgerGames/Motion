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
    private ulong _clientId;
    private NetworkObject _networkObject;
    private bool _hasCollided = false;

    public void Start()
    {
        Physics.IgnoreLayerCollision(7, 7);
    }
    // SetupAndSpawn sets up the projectile, and creates it on the network
    public void SetupAndSpawn(float speed, float damage, ulong clientId)
    {
        _speed = speed;
        _damage = damage;
        _clientId = clientId;
        _networkObject = gameObject.GetComponent<NetworkObject>();
        _networkObject.CheckObjectVisibility = ((clientIdToCheck) =>
        {
            if (!IsServer && clientIdToCheck == _clientId)
            {
                return false;
            }
            else
            {
                return true;
            }
        });
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
        if (!IsServer || _hasCollided) return;
        _hasCollided = true;
        

        Debug.Log("Projectile collided with " + collision.gameObject.name + " At location " + gameObject.transform.position);
        if (collision.gameObject.tag == "Player")
        {
            GamePlayerManager player = collision.transform.parent.GetComponent<GamePlayerManager>(); // TODO Improve player structure/reference to remove this hack bit?
            player.ModifyHealthServerRpc(- _damage);
        }
        
        _networkObject.Despawn();    
    }
}
