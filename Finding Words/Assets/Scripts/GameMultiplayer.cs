using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class GameMultiplayer : SingletonNetwork<GameMultiplayer>
{
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
    private NetworkList<PlayerData> playerDataNetworkList;


    public event EventHandler PlayerDataNetworkListChangedEvent;

    public event EventHandler OnFailedToJoinGame;

    private string playerName;
    public override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += OnListChanged;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000)); 
    }

    private void OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        PlayerDataNetworkListChangedEvent?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
        NetworkManager.Singleton.StartHost();
    }

    private void AddPlayer(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId
        });
        SetPlayer(clientId);
    }
    private void RemovePlayer(ulong clientId)
    {
        for(int i = 1; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }


    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void StartClient()
    {
        NetworkManager.OnClientConnectedCallback += SetPlayer;
        NetworkManager.OnClientDisconnectCallback += FailedToJoinGame;
        NetworkManager.Singleton.StartClient();
    }

    private void FailedToJoinGame(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void SetPlayer(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIdx = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIdx];
        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIdx] = playerData;
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIdx = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIdx];
        playerData.playerId = playerId;
        playerDataNetworkList[playerDataIdx] = playerData;
    }


    public bool IsPlayerIndexConnected(int playerIdx)
    {
        return playerIdx < playerDataNetworkList.Count;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        RemovePlayer(clientId);
    }

    public int GetNumberPlayers()
    {
        return playerDataNetworkList.Count;
    }

}
