using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ViewController : NetworkBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;
    [SerializeField] private GameObject PlayerObject;

    float xRotation;
    float yRotation; 

    // Start is called before the first frame update
    void Start()
    {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
