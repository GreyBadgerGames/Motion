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
    private NetworkObject _no;

    public void CreateAndSpawn(float speed, float damage)
    {
        if (!IsServer) return;

        _speed = speed;
        _damage = damage;
        _no = 
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (!IsOwner) return;
        // gameObject destroy
    }


    [ServerRpc]
    public void 
}
