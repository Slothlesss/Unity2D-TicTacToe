using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;


public class GridManager : NetworkBehaviour
{

    Button[,] buttons = new Button[3, 3];

    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite ySprite;

    private int[,] gridState = new int[3, 3];

    void Start()
    {
        var grids = GetComponentsInChildren<Button>();
        int n = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buttons[i, j] = grids[n];
                gridState[i, j] = 0;
                n++;

                int r = i;
                int c = j;
                buttons[i, j].onClick.AddListener(() =>
                {
                    ClickGrid(r, c);
                });
            }
        }
    }

    private void ClickGrid(int r, int c)
    {
        if (NetworkManager.Singleton.IsHost && GameManager.Instance.GetCurrentTurn().Value == 0)
        {
            //Server call this to execute on all client
            ChangeSpriteClientRpc(r , c); //Executed on all clients. Because host is also client, therefore we don't need to call this one more time;
            GameManager.Instance.SetCurrentTurn(1);
            gridState[r, c] = 1;
        }
        else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.GetCurrentTurn().Value == 1)
        {
            //When Client call ServerRPc function, it's only the call site, no execution on client side.
            ChangeSpriteServerRpc(r, c); //Only executed on host/server
            //Therefore we need to call this on client one more time to synchonize both side.
            buttons[r, c].GetComponent<SingleGrid>().ShowY();
            //Because we can't change the currentTurn value on Client, we put the Set function in ServerRpc.
            gridState[r, c] = 10;
        }
        else
        {
            return;
        }
        ShowResult(r, c);
        buttons[r, c].interactable = false;
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int r, int c)
    {
        buttons[r, c].GetComponent<SingleGrid>().ShowX();
        buttons[r, c].interactable = false;
        gridState[r, c] = 1;
    }

    [ServerRpc(RequireOwnership = false)] 
    private void ChangeSpriteServerRpc(int r, int c)
    {
        buttons[r, c].GetComponent<SingleGrid>().ShowY();
        buttons[r, c].interactable = false;
        gridState[r, c] = 10;
        GameManager.Instance.SetCurrentTurn(0);
    }

    private int CheckResult(int r, int c)
    {
        int sumRow = gridState[r, 0] + gridState[r, 1] + gridState[r, 2];
        int sumCol = gridState[0, c] + gridState[1, c] + gridState[2, c];
        int sumCross1 = gridState[0, 0] + gridState[1, 1] + gridState[2, 2];
        int sumCross2 = gridState[0, 2] + gridState[1, 1] + gridState[2, 0];
        if (sumRow == 3 || sumCol == 3 || sumCross1 == 3 || sumCross2 == 3)
        {
            return 1;
        }
        else if (sumRow == 30 || sumCol == 30 || sumCross1 == 30 || sumCross2 == 30)
        {
            return 2;
        }
        else if (IsDraw())
        {
            return 3;
        }
        
        return 0;
    }

    private bool IsDraw()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (gridState[i, j] == 0) return false;
            }
        }
        return true;
    }

    private void ShowResult(int r, int c)
    {
        if (CheckResult(r, c) != 0)
        {
            GameManager.Instance.ShowResult(CheckResult(r, c));
        }
    }
}
