using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireScript : NetworkBehaviour

    
{
    bool requestFire;
    public Transform orientation;

    [SerializeField] private GameObject ProjectileObject;
    [SerializeField] private GameObject PlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        requestFire = playerInput();
        if (requestFire == true) { 
            fireServerRpc();
            requestFire = false;
        }
    }

    void FixedUpdate()
    {
        
    }


    bool playerInput() {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("pew");
            return true;
            
        }
        return false;
    }
    [ServerRpc]
    void fireServerRpc()
    {
         Debug.Log("Spawn shot server mode!");
         GameObject projectile = Instantiate(ProjectileObject, PlayerCamera.transform.position + new Vector3(0,0,0), PlayerCamera.transform.rotation);
         projectile.GetComponent<NetworkObject>().Spawn();
    }

}
