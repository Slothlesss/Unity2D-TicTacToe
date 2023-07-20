using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyMessage : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        GameMultiplayer.Instance.OnFailedToJoinGame += FailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted += CreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += CreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += JoinStarted;
        GameLobby.Instance.OnJoinFailed += JoinFailed;
        GameLobby.Instance.OnQuickJoinFailed += QuickJoinFailed;

        Hide();
    }

    private void QuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void JoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
    }

    private void JoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void CreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void CreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void FailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
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
        GameMultiplayer.Instance.OnFailedToJoinGame -= FailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted -= CreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed -= CreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted -= JoinStarted;
        GameLobby.Instance.OnJoinFailed -= JoinFailed;
        GameLobby.Instance.OnQuickJoinFailed -= QuickJoinFailed;
    }

}