using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    [SerializeField] private Slider _healthBarSlider;
    [SerializeField] private GameObject _settingsModal;
    [SerializeField] private Canvas _scoreboard;
    private GamePlayerManager m_localGamePlayerManager;

    void Start()
    {
        _settingsModal.SetActive(false);
    }

    void Update()
    {
        UpdateHealthBar();
        CheckUIRequests();
    }

    private void UpdateHealthBar()
    {
        // TODO Move getting this component to an "OnPlayerSpawned" esque function. Not sure how to do that from here
        PersistentPlayerManager localPersistentPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>();
        m_localGamePlayerManager = localPersistentPlayer.gamePlayer.GetComponent<GamePlayerManager>();
        if (m_localGamePlayerManager != null)
        {
            _healthBarSlider.value = m_localGamePlayerManager._health.Value/100;
        }
    }

    // CheckUIRequests checks if the user wants to see special UI, and lets the GamePlayerManager know
    // if it is free to check for movements
    private void CheckUIRequests()
    {
        bool showCursor = false;
        bool allowMovement = true;
        
        if (requestSettingsModal())
        {
            // Settings open, show cursor and don't move character
            showCursor = true;
            allowMovement = false;
        }
        else if (requestScoreboard())
        {
            // Scoreboard open, show cursor and allow character move
            showCursor = false; // TODO Add button to allow mouse?
            allowMovement = true;
        }

        // Let the gamePlayerManager know if it is free to check for moves
        m_localGamePlayerManager._canMove = allowMovement;
        
        Cursor.visible = showCursor;
        if (showCursor) {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // requestSettingsModal returns true if the TAB key is pressed,
    // and if requesting to open it, returns true and sets it active
    private bool requestScoreboard()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (!_scoreboard.enabled) _scoreboard.enabled = true; // If not active, set it active
            return true;
        }

        _scoreboard.enabled = false;
        return false;
    }

    // requestSettingsModal returns true if the _settingsModal is active,
    // and if requesting to open it, returns true and sets it active
    private bool requestSettingsModal()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _settingsModal.SetActive(true);
            return true;
        }

        return _settingsModal.activeInHierarchy;
    }
}
