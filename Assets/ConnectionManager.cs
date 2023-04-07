using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectionManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text _nameInputField;

    // ConnectionManager is spawned when the player joins (or creates) a server
    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty("MainMenu"))
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>().SetName(_nameInputField.text);

            Debug.Log("Changing scene to Lobby");
            var status = NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load Game " +
                    $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}
