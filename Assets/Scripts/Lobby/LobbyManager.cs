using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private GameObject _settingsModal;
    [SerializeField] private Button _exitLobbyBtn;
    [SerializeField] private Button _readyButton;
    [SerializeField] private TMP_Text _lobbyList;
    [SerializeField] private TMP_Dropdown _gameModeDropdown;
    [SerializeField] public List<GameMode> _gameModePrefabs;
    [SerializeField] private TMP_Text _gameModeDescription;
    public NetworkVariable<int> _selectedGameMode = new NetworkVariable<int>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private bool _ready;
    private int _lastLocalGameModeSelection = 0;

    public override void OnNetworkSpawn()
    {
        _settingsModal.SetActive(false);

        UpdateLobbyListServerRpc(NetworkManager.LocalClientId, _ready);

        _readyButton.onClick.AddListener(readyButtonClicked);
        _exitLobbyBtn.onClick.AddListener(exitLobbyButtonClicked);
        _settingsBtn.onClick.AddListener(settingsButtonClicked);
        initGameModeDropdown();
        base.OnNetworkSpawn();
    }

    private void readyButtonClicked()
    {
        _ready = !_ready;
        UpdateLobbyListServerRpc(NetworkManager.LocalClientId, _ready);
        CheckAllPlayersReadyServerRpc();
    }

    private void exitLobbyButtonClicked()
    {
        if (IsHost) // TODO Remove - hack for host local testing
        {
            SceneManager.LoadScene("MainMenu");
            NetworkManager.Singleton.Shutdown();
            return;
        }

        Debug.LogWarning($"Requesting to disconnect {NetworkManager.LocalClientId}");
        disconnectClientServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc (RequireOwnership = false)]
    private void disconnectClientServerRpc(ulong clientId)
    {
        Debug.LogWarning($"Disconnecting {clientId}");
        NetworkManager.Singleton.DisconnectClient(clientId);
    }

    private void settingsButtonClicked()
    {
        _settingsModal.SetActive(true);
    }

    private void initGameModeDropdown()
    {
        _gameModeDropdown.options.Clear();
        foreach (GameMode gm in _gameModePrefabs)
        {
            _gameModeDropdown.options.Add(new TMP_Dropdown.OptionData() {text = gm._name});
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateSelectedGameModeServerRpc(int gameModeIndex)
    {
        _selectedGameMode.Value = gameModeIndex;
    }

    void FixedUpdate()
    {
        if (_gameModeDropdown.value != _lastLocalGameModeSelection)
        {
            // A local change has happened
            UpdateSelectedGameModeServerRpc(_gameModeDropdown.value);
        }
        _lastLocalGameModeSelection = _selectedGameMode.Value;
        _gameModeDropdown.value = _selectedGameMode.Value;
        _gameModeDescription.text = _gameModePrefabs[_selectedGameMode.Value]._decription;
        

        if (!IsServer) return;
        string text = "";
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayerManager player = client.Value.PlayerObject.GetComponent<PersistentPlayerManager>();
            if (player.readyInLobby.Value)
            {
                text += player.GetName() + " - READY\n";
                continue;
            }
            text += player.GetName() + " - WAITING\n";
        }

        updateLobbyListTextClientRpc(text);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckAllPlayersReadyServerRpc()
    {
        int numReady = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayerManager player = client.Value.PlayerObject.GetComponent<PersistentPlayerManager>();
            if (player.readyInLobby.Value)
            {
                numReady ++;
            }
        }

        if (numReady == NetworkManager.Singleton.ConnectedClients.Count) {
            LoadGame();
        }
    }

    private void LoadGame()
    {
        // Create the GameMode
        var gameMode = Instantiate(_gameModePrefabs[_selectedGameMode.Value]);
        gameMode.GetComponent<NetworkObject>().Spawn();
        
        var status = NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load Game " +
                $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }

    // UpdateConnectedClients updates the LobbyList's local cache of connected playerNames
    [ServerRpc(RequireOwnership = false)]
    public void UpdateLobbyListServerRpc(ulong clientId, bool ready)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PersistentPlayerManager>().readyInLobby.Value = ready;
    }

    [ClientRpc]
    private void updateLobbyListTextClientRpc(string text)
    {
        _lobbyList.text = text;
    }
}
