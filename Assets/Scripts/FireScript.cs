using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireScript : NetworkBehaviour
{
    bool requestFire;
    public Transform orientation;

    [SerializeField] private GameObject ProjectileObject;
    [SerializeField] private GameObject LocalProjectileObject;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileDamage;
    private Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        requestFire = playerInput();
        if (requestFire == true) { 
            //FireServerRpc(
            //    PlayerCamera.transform.position + PlayerCamera.transform.forward * 0.75f,
            //    PlayerCamera.transform.rotation,
            //    PlayerCamera.transform.forward
            //);
            FireLocal(
                PlayerCamera.transform.position + PlayerCamera.transform.forward * 0.9f,
                PlayerCamera.transform.rotation,
                PlayerCamera.transform.forward
            );
            requestFire = false;
        }
    }

    bool playerInput() {
        return Input.GetMouseButtonDown(0);
    }
    
    [ServerRpc]
    void FireServerRpc(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection)
    {
        Debug.Log("Spawn shot server mode!");
        GameObject projectileInstance = Instantiate(ProjectileObject, spawnPos, spawnRotation);
        ProjectileManager networkProjectile = projectileInstance.GetComponent<ProjectileManager>();
        networkProjectile.SetupAndSpawn(_projectileSpeed, _projectileDamage);
        networkProjectile.Fire(fireDirection);
    }

    void FireLocal(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection)
    {
        GameObject localProjectileInstance = Instantiate(LocalProjectileObject, spawnPos, spawnRotation);
        LocalProjectileManager localProjectile = localProjectileInstance.GetComponent<LocalProjectileManager>();
        localProjectile.Setup(_projectileSpeed, _projectileDamage);
        localProjectile.Fire(fireDirection);
    }

}
