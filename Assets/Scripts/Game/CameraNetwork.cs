using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject cameraHolder;

    void Start()
    {
        cameraHolder.SetActive(IsOwner);
    }
}