using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PositionHistoryManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerObject;

    public Vector3[] positionHistory = new Vector3[3];
    public Vector3 pos1;
    public Vector3 pos2;


    void Start()
    { 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePositionHistoryServerRpc(_playerObject.transform.position);
        //SpeedSqrMagnitudeCheck();
    }

    [ServerRpc]
    void UpdatePositionHistoryServerRpc(Vector3 currentPosition)
    {
        if (!IsServer) return;
        //Loop through array and shift values one position left
        for (int i = 0; i < positionHistory.Length; i++)
        {
            if (i != 0)
            {
                positionHistory[i-1] = positionHistory[i];
            }
            if (i == (positionHistory.Length - 1))
            {
                positionHistory[i] = currentPosition; 
            }
        }
    }

//Prints Square magnitude of position differences to console for debug
    void SpeedSqrMagnitudeCheck()
    {
        pos1 = positionHistory[2];
        pos2 = positionHistory[1];
        Vector3 vector3 = pos1 - pos2;
        float sqrLen = vector3.sqrMagnitude;
        Debug.Log(sqrLen);
    }

    public Vector3[] GetPositionHistory()
    {
        return positionHistory;
    }
}
