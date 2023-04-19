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
            Fire(
                PlayerCamera.transform.position + PlayerCamera.transform.forward,
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
        //Debug.Log("Server shot from " + clientId);
        GameObject projectileInstance = Instantiate(ProjectileObject, spawnPos, spawnRotation);
        ProjectileManager networkProjectileManager = projectileInstance.GetComponent<ProjectileManager>();
        Physics.IgnoreCollision(projectileInstance.transform.GetChild(0).GetComponent<Collider>(), gameObject.transform.GetChild(0).GetComponent<Collider>());
        networkProjectileManager.SetupAndSpawn(_projectileSpeed, _projectileDamage, clientId);
        networkProjectileManager.Fire(fireDirection);
    }

    void FireLocal(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection, ulong clientId)
    {
        //Debug.Log("client shot from " + clientId);
        GameObject localProjectileInstance = Instantiate(LocalProjectileObject, spawnPos, spawnRotation);
        LocalProjectileManager localProjectileManager = localProjectileInstance.GetComponent<LocalProjectileManager>();
        Physics.IgnoreCollision(localProjectileInstance.transform.GetChild(0).GetComponent<Collider>(), gameObject.transform.GetChild(0).GetComponent<Collider>());
        localProjectileManager.Setup(_projectileSpeed, _projectileDamage);
        localProjectileManager.Fire(fireDirection);
    }

    void Fire(Vector3 spawnPos, Quaternion spawnRotation, Vector3 fireDirection, ulong clientId)
    {
        FireLocal(spawnPos, spawnRotation, fireDirection, clientId);
        FireServerRpc(spawnPos, spawnRotation, fireDirection, clientId); 
    }
}
