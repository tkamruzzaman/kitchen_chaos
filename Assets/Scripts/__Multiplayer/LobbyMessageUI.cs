using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            FindObjectOfType<LobbyUI>().SetDefaultSelectedButtonForGamepad();
            Hide();
        });
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnFailedToJoinGame += MultiplayerGameManager_OnFailedToJoinGame;

        LobbyManager.Instance.OnCreateLobbyStarted += LobbyManager_OnCreateLobbyStarted;
        LobbyManager.Instance.OnCreateLobbyFailed += LobbyManager_OnCreateLobbyFailed;
        LobbyManager.Instance.OnJoinStarted += LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnJoinFailed;
        LobbyManager.Instance.OnQuickJoinFailed += LobbyManager_OnQuickJoinFailed;

        Hide();
    }

    private void LobbyManager_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void LobbyManager_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join lobby!");
    }

    private void LobbyManager_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a lobby to Quick join!");
    }

    private void LobbyManager_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void LobbyManager_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create lobby!");
    }

    private void MultiplayerGameManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        string disconnectReason = NetworkManager.Singleton.DisconnectReason;

        ShowMessage(!string.IsNullOrEmpty(disconnectReason)
            ? disconnectReason : "Failed to connect!");
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
        closeButton.Select();
    }

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnFailedToJoinGame -= MultiplayerGameManager_OnFailedToJoinGame;

        LobbyManager.Instance.OnCreateLobbyStarted -= LobbyManager_OnCreateLobbyStarted;
        LobbyManager.Instance.OnCreateLobbyFailed -= LobbyManager_OnCreateLobbyFailed;
        LobbyManager.Instance.OnJoinStarted -= LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnJoinFailed -= LobbyManager_OnJoinFailed;
        LobbyManager.Instance.OnQuickJoinFailed -= LobbyManager_OnQuickJoinFailed;
    }
}