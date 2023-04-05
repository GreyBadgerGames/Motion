using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

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
    }

    // hostButton should only be called for testing
    private void hostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void clientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
}
