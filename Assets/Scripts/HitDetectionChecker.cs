using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HitDetectionChecker : NetworkBehaviour
{


    // Update is called once per frame
    void Update()
    {
        
    }
    public bool HitDetection(Vector3 playerHitPosition, Vector3[] playerLocationHistory)
    {
        foreach (Vector3 i in playerLocationHistory)
        {
            Vector3 vectordiff = playerHitPosition - i;
            float locationDiff = vectordiff.sqrMagnitude;

            if (locationDiff < 0.025) {
                return true;
            }
        }
        return false;
    }
}
