using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class MovementScript : NetworkBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    public float maxSpeed;
    public Transform orientation;

    float horizontalInput;

    float verticalInput;

    float sprintMultiplier;

    Vector3 moveDirection;

    private Rigidbody rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        Debug.Log(horizontalInput + " " + verticalInput);
    }

    float CheckSprint()
    {
        if (Input.GetKeyDown("left shift")) {
            sprintMultiplier = 3;
        }
        else
        {
            sprintMultiplier = 1;
        }
    return sprintMultiplier;
    }

    
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + horizontalInput * orientation.right;

        if (moveDirection == Vector3.zero)
        {
            rb.drag = 10;
        }
        else
        {
            rb.drag = (movementSpeed / maxSpeed);
        }

        rb.AddForce(moveDirection.normalized * (movementSpeed * CheckSprint()), ForceMode.Force);
    }
        

    
}

