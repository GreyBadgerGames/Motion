using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    [SerializeField] private Slider m_healthBarSlider;
    private GamePlayerManager m_localGamePlayerManager;

    void Start()
    {

    }

    void Update()
    {
        // TODO Move getting this component to an "OnPlayerSpawned" esque function. Not sure how to do that from here
        PersistentPlayerManager localPersistentPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PersistentPlayerManager>();
        m_localGamePlayerManager = localPersistentPlayer.gamePlayer.GetComponent<GamePlayerManager>();
        if (m_localGamePlayerManager != null)
        {
            m_healthBarSlider.value = m_localGamePlayerManager._health.Value/100;
        }
    }
}
