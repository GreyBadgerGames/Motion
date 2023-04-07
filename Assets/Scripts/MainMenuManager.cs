using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _exitToDesktopBtn;
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;
    [SerializeField] public TMP_InputField _nameField;
    [SerializeField] public TMP_InputField _ipField;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(hostButtonClicked);
        _clientBtn.onClick.AddListener(clientButtonClicked);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    // hostButton should only be called for testing
    private void hostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void clientButtonClicked()
    {
        if (_ipField.text != "")
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = _ipField.text;
        }
        NetworkManager.Singleton.StartClient();

    }

    void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client connected, clientId: {clientId}");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>().SetNameServerRpc(_nameField.text);
    }
}
