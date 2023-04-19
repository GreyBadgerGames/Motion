using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    void Start()
    {
        // TODO #18 check if server
        SceneManager.LoadScene("MainMenu");
    }
}
