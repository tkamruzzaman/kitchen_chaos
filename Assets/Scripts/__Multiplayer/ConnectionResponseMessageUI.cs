using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnFailedToJoinGame += MultiplayerGameManager_OnFailedToJoinGame;
        Hide();
    }

    private void MultiplayerGameManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();

        string disconnectReason = NetworkManager.Singleton.DisconnectReason;

        messageText.text = 
            !string.IsNullOrEmpty(disconnectReason) 
            ? disconnectReason : "Failed to connect!";
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnFailedToJoinGame -= MultiplayerGameManager_OnFailedToJoinGame;
    }
}