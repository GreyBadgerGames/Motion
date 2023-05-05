using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileManager : NetworkBehaviour
{
    private float _speed;
    private float _damage;
    private ulong _clientId;
    private NetworkObject _networkObject;
    private bool _hasCollided = false;
    private double k_physicsUpdateTimeSeconds = 0.02;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Transform _transform;
    private Rigidbody _rigidBody;
    
    public void Update()
    {
        // Rotate the collider and set the length based on the current projectile speed
        transform.rotation = Quaternion.LookRotation(_rigidBody.velocity);
        GenericProjectile.SetColliderLength(_rigidBody, _transform, _collider, k_physicsUpdateTimeSeconds);
    }
    
    // SetupAndSpawn sets up the projectile, and creates it on the network
    public void SetupAndSpawn(float speed, float damage, ulong clientId)
    {
        // Setup vars
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _networkObject = gameObject.GetComponent<NetworkObject>();
        _speed = speed;
        _damage = damage;
        _clientId = clientId;
        _rigidBody.isKinematic = false;
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
       
        Physics.IgnoreLayerCollision(7, 7); // Ensure projectiles don't hit others

        _networkObject.Spawn();
    }

    // Fire adds an impulse force to the projectile's RigidBidy as an impluse
    public void Fire(Vector3 direction)
    {
        _rigidBody.AddForce(direction * _speed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (!IsServer || _hasCollided) return;
        _hasCollided = true;
        
        _networkObject.Despawn();    
    } 
}

// GenericProjectile defines methods to be used by both Network and Local projectile types
public class GenericProjectile : MonoBehaviour
{
    // SetColliderLength calculates the minimum length of the projectile collider, based off the current velocity.
    // This ensures the projectile's collider cannot completely skip over a single pixel in any given physics update.
    public static void SetColliderLength(Rigidbody rb, Transform tf, CapsuleCollider collider, double physicsUpdateDelaySeconds)
    {
        // Given: s = projectile speed (units per second), p = physics time (seconds), l = projectile length (units)
        
        // Calculate length l = s * p and add a tiny bit extra
        double length = rb.velocity.magnitude * physicsUpdateDelaySeconds + 0.001;

        // Collider height is relative to the localScale of the GameObject
        collider.height = (float)(length * 1 / tf.localScale.z);
        collider.direction = 2; //  0, 1 or 2 = X, Y and Z axes
        
        // Shuffle the collider so the front is inline with the front of the projectile
        collider.center = new Vector3(0,0, -(float)(collider.height/2 - collider.radius));
    }  
}
