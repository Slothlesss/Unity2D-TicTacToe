using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIdx;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshProUGUI playerName;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIdx);
            GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            GameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }
    private void Start()
    {
        GameMultiplayer.Instance.PlayerDataNetworkListChangedEvent += PlayerDataNetworkListChanged;
        CharacterSelectReady.OnReadyChangedEvent += OnReadyChanged;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void OnReadyChanged()
    {
        UpdatePlayer();
    }
    private void PlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIdx))
        {
            Show();
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIdx);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerName.text = playerData.playerName.ToString();
        } 
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.PlayerDataNetworkListChangedEvent -= PlayerDataNetworkListChanged;
    }
}
