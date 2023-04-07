using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireBehaviour : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject PlayerCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // rb = GetComponent<Rigidbody>();
      //  rb.isKinematic = false;
      //  rb.AddForce(PlayerCamera.transform.forward * 100, ForceMode.Force);
    }
}
