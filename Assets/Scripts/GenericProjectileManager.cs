using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// GenericProjectile defines methods to be used by both Network and Local projectile types
public abstract class GenericProjectileManager : NetworkBehaviour
{
    protected float _speed;
    protected float _damage;
    protected bool _hasCollided = false;
    protected double k_physicsUpdateTimeSeconds = 0.02;
    [SerializeField] protected CapsuleCollider _collider;
    [SerializeField] protected Transform _transform;
    protected Rigidbody _rigidBody;
    
    public abstract void Update();
    public abstract void OnCollisionEnter(Collision collision);
    public abstract void Spawn(ulong clientId);
    
    // Setup sets up the projectile
    public void Setup(float speed, float damage)
    {
        // Setup vars
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _speed = speed;
        _damage = damage;
        _rigidBody.isKinematic = false;

        Physics.IgnoreLayerCollision(7, 7); // Ensure projectiles don't hit others
    }

    public void Fire(Vector3 direction)
    {
        _rigidBody.AddForce(direction * _speed, ForceMode.Impulse);
    }

    // SetColliderLength calculates the minimum length of the projectile collider, based off the current velocity.
    // This ensures the projectile's collider cannot completely skip over a single pixel in any given physics update.
    public void SetColliderLength()
    {
        // Given: s = projectile speed (units per second), p = physics time (seconds), l = projectile length (units)
        
        // Calculate length l = s * p and add a tiny bit extra
        double length = _rigidBody.velocity.magnitude * k_physicsUpdateTimeSeconds + 0.001;

        // Collider height is relative to the localScale of the GameObject
        _collider.height = (float)(length * 1 / _transform.localScale.z);
        _collider.direction = 2; //  0, 1 or 2 = X, Y and Z axes
        
        // Shuffle the collider so the front is inline with the front of the projectile
        _collider.center = new Vector3(0,0, -(float)(_collider.height/2 - _collider.radius));
    }

    // TODO Stop this spamming logs "Look rotation viewing vector is zero"
    // SetColliderRotation rotates the collider and set the length based on the current projectile speed
    protected void SetColliderRotation()
    {
        transform.rotation = Quaternion.LookRotation(_rigidBody.velocity);
    }
}
