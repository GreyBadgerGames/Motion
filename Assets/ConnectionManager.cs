using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ConnectionManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty("MainMenu"))
        {
            var status = NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load Game " +
                    $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}
