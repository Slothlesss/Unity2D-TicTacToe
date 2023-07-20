using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class SingleGrid : MonoBehaviour
{
    [SerializeField] private GameObject xObject;
    [SerializeField] private GameObject yObject;
    [SerializeField] private int currentState = 0;

    public void ShowX()
    {
        xObject.SetActive(true);
        yObject.SetActive(false);
        currentState = 1;
    }

    public void ShowY()
    {
        xObject.SetActive(false);
        yObject.SetActive(true);
        currentState = 10;
    }

    public int GetCurrentState()
    {
        return currentState;
    }


}
