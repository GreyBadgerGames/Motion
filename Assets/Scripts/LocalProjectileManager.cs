using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LocalProjectileManager : GenericProjectileManager
{
    public override void Update()
    {
        SetColliderRotation();
        SetColliderLength();
    }

    public override void Spawn(ulong clientId)
    {
        // TODO Maybe use a "virtual" method here?
        throw new System.NotImplementedException();
    }

    public override void OnCollisionEnter(Collision collision) 
    {
        if (_hasCollided) return;
        _hasCollided = true;

        Debug.Log("Local projectile collided with " + collision.gameObject.name + " At location " + gameObject.transform.position);
        if (collision.gameObject.tag == "Player")
        {
            GamePlayerManager player = collision.gameObject.GetComponent<GamePlayerManager>();
            Debug.Log("Trying to get GamePlayerManager: " + player);
            player.RequestHitServerRpc(-_damage, NetworkManager.Singleton.LocalClientId, collision.gameObject.transform.position);
        }

        Destroy(gameObject);
    }
}
