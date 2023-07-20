using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class UILobby : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private GameObject mainButtonField;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button newRoomButton;

    [Header("Join Room")]
    [SerializeField] private GameObject joinRoomField;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;

    [Header("New Room")]
    [SerializeField] private GameObject newRoomField;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;


    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        joinRoomButton.onClick.AddListener(() =>
        {
            ShowField(joinRoomField);
        });
        newRoomButton.onClick.AddListener(() =>
        {
            ShowField(newRoomField);
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        backButton.onClick.AddListener(() =>
        {
            if (mainButtonField.activeInHierarchy)
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            } 
            else
            {
                ShowField(mainButtonField);
            }
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayer.Instance.SetPlayerName(newText);
        });

        GameLobby.Instance.OnLobbyListChanged += LobbyListChanged;
        UpdateLobbyList(new List<Lobby>());

        ShowField(mainButtonField);
    }

    private void ShowField(GameObject buttonField)
    {
        mainButtonField.SetActive(false);
        joinRoomField.SetActive(false);
        newRoomField.SetActive(false);
        buttonField.SetActive(true);
    }

    private void LobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach(Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<UILobbyListSingle>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        GameLobby.Instance.OnLobbyListChanged -= LobbyListChanged;
    }

}
