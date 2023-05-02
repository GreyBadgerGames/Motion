using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHistoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerObject;
    public Vector3[] positionHistory = new Vector3[44];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePositionHistory(_playerObject.transform.position);
        Debug.Log(positionHistory[0]);
    }

    void UpdatePositionHistory(Vector3 currentPosition)
    {
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
}
