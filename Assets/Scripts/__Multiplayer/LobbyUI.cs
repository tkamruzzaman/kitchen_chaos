using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeWithButton;
    [SerializeField] private TMP_InputField joinWithCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [Space]
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    [Space]
    [SerializeField] private LobbyCreateUI createLobbyUI;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.QuickJoin();
        });

        joinCodeWithButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinWithCode(joinWithCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = MultiplayerGameManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            MultiplayerGameManager.Instance.SetPlayerName(newText);
        });

        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;

        UpdateLobbyList(new());
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }
}
