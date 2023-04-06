using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] public TMP_InputField nameField;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
                HideNetworkUI();
        });
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            if (GameObject.Find("Player(Clone)") != null)
            {
                HideNetworkUI();
            }
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            if (GameObject.Find("Player(Clone)") != null)
            {
                HideNetworkUI();
            }
        });
    }

    private void HideNetworkUI()
    {
        serverBtn.gameObject.SetActive(false);
        hostBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
    }

    private void ShowNetworkUI()
    {
        serverBtn.gameObject.SetActive(true);
        hostBtn.gameObject.SetActive(true);
        clientBtn.gameObject.SetActive(true);
    }
}
    