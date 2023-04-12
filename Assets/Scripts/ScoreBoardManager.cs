using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UIElements;
using Unity.Collections;

public class ScoreBoardManager : NetworkBehaviour
{
    [SerializeField] private Transform m_scrollViewContent;
    [SerializeField] private GameObject m_playerBarPrefab;

    void Start()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    void Update()
    {
        gameObject.GetComponent<Canvas>().enabled = Input.GetKey(KeyCode.Tab);

        if (!IsServer) return;
        Dictionary<string, string> playersDict = new Dictionary<string, string>();
        string playersString = "";
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            PersistentPlayerManager persistentPlayer = client.Value.PlayerObject.GetComponent<PersistentPlayerManager>();
            GamePlayerManager player = persistentPlayer.gamePlayer.GetComponent<GamePlayerManager>();
            playersString += $"{persistentPlayer.GetName()}:{player._health.Value},";
            // TODO sort by score/health
        }
        UpdateScoreBoardClientRpc(playersString);
    }

    // playerString takes the form "player1:score,player2:score"...
    [ClientRpc]
    private void UpdateScoreBoardClientRpc(string playerString)
    {
        if (!Input.GetKey(KeyCode.Tab)) return; // Tab not pressed so don't actually need to update anything
        foreach (RectTransform oldBar in m_scrollViewContent.transform)
        { 
            Destroy(oldBar.gameObject);             
        }

        string[] players = playerString.ToString().Split(",");
        foreach (var player in players)
        {
            if (player == "") continue;
            string[] playerScore = player.Split(":");
            var playerBar = Instantiate(m_playerBarPrefab, m_scrollViewContent);
            playerBar.transform.GetChild(0).GetComponent<TMP_Text>().text = playerScore[0];
            playerBar.transform.GetChild(1).GetComponent<TMP_Text>().text = playerScore[1];
        }
    }
}
