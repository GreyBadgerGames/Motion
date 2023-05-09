using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _exitToDesktopBtn;
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] public TMP_InputField _nameField;
    [SerializeField] public TMP_InputField _ipField;
    [SerializeField] public GameObject _settingsModal;

    private void Start()
    {
        // Hack to allow starting the game in editor from Game scene :)
        if (NetworkManager.Singleton == null) SceneManager.LoadScene("Startup");
    }

    private void Awake()
    {    
        _hostBtn.onClick.AddListener(hostButtonClicked);
        _clientBtn.onClick.AddListener(clientButtonClicked);
        _exitToDesktopBtn.onClick.AddListener(exitToDesktopButtonClicked);
        _settingsBtn.onClick.AddListener(settingsButtonClicked);
        _settingsModal.SetActive(false);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void Update()
    {
        _clientBtn.enabled = !(_nameField.text == "");
        _hostBtn.enabled = !(_nameField.text == "");
    }

    private void exitToDesktopButtonClicked()
    {
        Application.Quit();
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

    private void settingsButtonClicked()
    {
        _settingsModal.SetActive(true);
    }

    void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client connected, clientId: {clientId}");
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>().SetNameServerRpc(_nameField.text);
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}
