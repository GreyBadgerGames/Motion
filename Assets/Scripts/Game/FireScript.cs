using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireScript : NetworkBehaviour
{
    bool requestFire;
    public Transform orientation;

    [SerializeField] private GameObject NetworkProjectileObject;
    [SerializeField] private GameObject LocalProjectileObject;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileDamage;
    [SerializeField] private GamePlayerManager gamePlayerManager;

    void Update()
    {
        if (!IsOwner) return;
        requestFire = playerInput();
        if (requestFire == true && gamePlayerManager._canMove) { 
            Fire(
                PlayerCamera.transform.position + PlayerCamera.transform.forward * 0.1f,
                PlayerCamera.transform.rotation,
                PlayerCamera.transform.forward,
                NetworkManager.LocalClientId
            );
            requestFire = false;
        }
    }

    bool playerInput() {
        return Input.GetMouseButtonDown(0);
    }
    
    [ServerRpc]
    void FireServerRpc(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection, ulong clientId)
    {
        GameObject projectileInstance = Instantiate(NetworkProjectileObject, spawnPos, spawnRotation);
        NetworkProjectileManager networkProjectileManager = projectileInstance.GetComponent<NetworkProjectileManager>();

        ignoreSelfCollision(projectileInstance);

        networkProjectileManager.Setup(_projectileSpeed, _projectileDamage);
        networkProjectileManager.Spawn(clientId);
        networkProjectileManager.Fire(fireDirection);
    }

    void FireLocal(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection, ulong clientId)
    {
        GameObject projectileInstance = Instantiate(LocalProjectileObject, spawnPos, spawnRotation);
        LocalProjectileManager localProjectileManager = projectileInstance.GetComponent<LocalProjectileManager>();

        ignoreSelfCollision(projectileInstance);

        localProjectileManager.Setup(_projectileSpeed, _projectileDamage);
        localProjectileManager.Fire(fireDirection);
    }

    void Fire(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection, ulong clientId)
    {
        FireLocal(spawnPos, spawnRotation, fireDirection, clientId);
        FireServerRpc(spawnPos, spawnRotation, fireDirection, clientId); 
    }

    private void ignoreSelfCollision(GameObject projectile)
    {
        Physics.IgnoreCollision(projectile.transform.GetChild(0).GetComponent<Collider>(), gameObject.transform.GetChild(0).GetComponent<Collider>());
    }
}
