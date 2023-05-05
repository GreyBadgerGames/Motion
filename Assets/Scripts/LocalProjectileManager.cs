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

    public void Setup(float speed, float damage)
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _speed = speed;
        _damage = damage;
        _rigidBody.isKinematic = false;

        Physics.IgnoreLayerCollision(7, 7); // Ensure projectiles don't hit others
    }

    // Fire adds an impulse force to the projectile's RigidBidy as an impluse
    public void Fire(Vector3 direction)
    {
        _rigidBody.AddForce(direction * _speed, ForceMode.Impulse);
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
            PositionHistoryManager positionHistoryManager = collision.gameObject.GetComponent<PositionHistoryManager>();
            HitDetectionChecker hitDetectionChecker = collision.gameObject.GetComponent<HitDetectionChecker>();
            Vector3[] positionHistory = positionHistoryManager.GetPositionHistory();
            player.RequestHitServerRpc(-_damage, NetworkManager.Singleton.LocalClientId, positionHistory, collision.gameObject.transform.position);
        }

        Destroy(gameObject);
    }
}
