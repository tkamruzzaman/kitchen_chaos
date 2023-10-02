using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            print("***** HOST ****");
            MultiplayerGameManager.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() =>
        {
            print("----- CLIENT -----");
            MultiplayerGameManager.Instance.StartClient();
            Hide();
        });

        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}