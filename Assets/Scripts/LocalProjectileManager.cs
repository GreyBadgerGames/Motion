using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Maybe move Projectile to Pooled Dynamic Spawning if we see performance issues?
// https://docs-multiplayer.unity3d.com/netcode/current/basics/object-spawning/index.html#pooled-dynamic-spawning

// ProjectileManager manages a generic projectile, with projectiles stats being set by the firer
public class LocalProjectileManager : MonoBehaviour
{
    private float _speed;
    private float _damage;
    private bool _hasCollided = false;

    public void Start()
    {
        Physics.IgnoreLayerCollision(7, 7);
    }

// SetupAndSpawn sets up the projectile, and creates it on the network
    public void Setup(float speed, float damage)
    {
        _speed = speed;
        _damage = damage;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    // Fire adds an impulse force to the projectile's RigidBidy as an impluse
    public void Fire(Vector3 direction)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(direction * _speed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasCollided) return;
        _hasCollided = true;

        Debug.Log("Local projectile collided with " + collision.gameObject.name + " At location " + gameObject.transform.position);
        if (collision.gameObject.tag == "Player")
        {
            GamePlayerManager player = collision.gameObject.GetComponent<GamePlayerManager>();
            Debug.Log("Trying to get GamePlayerManager: " + player);
            player.ModifyHealthServerRpc(-_damage);
        }

        Destroy(gameObject);
    }    
    
}
