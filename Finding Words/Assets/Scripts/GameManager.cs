using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : SingletonNetwork<GameManager>
{
    [SerializeField]
    private NetworkVariable<int> currentTurn = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject grid;

    [SerializeField] private GameObject gameMessageCanvas;
    [SerializeField] private TextMeshProUGUI resultText;

    private GameObject newBoard;
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnBoard();
        }
    }

    private void SpawnBoard()
    {
        newBoard = Instantiate(grid, Vector3.zero, Quaternion.identity);
        newBoard.GetComponent<NetworkObject>().Spawn();
    }

    private void DespawnBoard()
    {
        newBoard.GetComponent<NetworkObject>().Despawn();
    }

    public NetworkVariable<int> GetCurrentTurn()
    {
        return currentTurn;
    }

    public void SetCurrentTurn(int value)
    {
        currentTurn.Value = value;
    }


    public void ShowResult(int winner)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            ShowResultClientRpc(winner); //Show to Client
            ShowResultServerRpc(winner); //Show to Host
        }
        else if (!NetworkManager.Singleton.IsHost)
        {
            ShowResultServerRpc(winner); //Show to Host

            //Show to Client
            gameMessageCanvas.SetActive(true);
            if (winner == 1)
            {
                resultText.text = "You Lose !!!";
            }
            else if (winner == 2)
            {
                resultText.text = "You Win !!!";
            }
            else
            {
                resultText.text = "Draw !!!";
            }
        }
    }

    [ClientRpc]
    private void ShowResultClientRpc(int winner)
    {
        if (IsHost) return;
        gameMessageCanvas.SetActive(true);
        if (winner == 1)
        {
            resultText.text = "You Lose !!!";
        }
        else if (winner == 2)
        {
            resultText.text = "You Win !!!";
        }
        else
        {
            resultText.text = "Draw !!!";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowResultServerRpc(int winner)
    {
        gameMessageCanvas.SetActive(true);
        if (winner == 1)
        {
            resultText.text = "You Win !!!";
        }
        else if (winner == 2)
        {
            resultText.text = "You Lose !!!";
        }
        else
        {
            resultText.text = "Draw !!!";
        }
    }

    public void Restart()
    {
        if (IsHost)
        {
            DespawnBoard();
            SpawnBoard();
            RestartClientRpc();
        }
        else
        {
            gameMessageCanvas.SetActive(false); 
            RestartServerRpc(); 
        }
    }

    [ClientRpc]
    private void RestartClientRpc()
    {
        gameMessageCanvas.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc()
    {
        gameMessageCanvas.SetActive(false);
        DespawnBoard();
        SpawnBoard();
    }
}
