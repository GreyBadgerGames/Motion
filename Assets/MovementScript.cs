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
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool jumpKeyPressed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.E;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

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
        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        PlayerInput();
        GroundCheck();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpKeyPressed = Input.GetKey(jumpKey);
    }

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }
  
    private void DragControl()
    {
        if (grounded)
        {
            
            if (moveDirection == Vector3.zero)
            {
                rb.drag = 9;
            }
            else
            {
                rb.drag = groundDrag;
            }
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void walkingMovement()
    {
        moveDirection = orientation.forward * verticalInput + horizontalInput * orientation.right;
        DragControl();
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 currentVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); 

        if (currentVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = currentVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);  
        }
    }

    private void Jump ()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump ()
    {
        readyToJump = true;
    }

    private void JumpMovement()
    {
        if (readyToJump && grounded && jumpKeyPressed)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void MovePlayer()
    {
        walkingMovement();
        JumpMovement();
    }
        

    
}

