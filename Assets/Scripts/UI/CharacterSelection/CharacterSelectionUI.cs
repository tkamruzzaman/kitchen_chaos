using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyCodeText;

    private CharacterSelectionManager characterSelectionManager;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.LoadScene(Loader.Scene.MainMenuScene);
        });

        readyButton.onClick.AddListener(() =>
        {
            characterSelectionManager = FindObjectOfType<CharacterSelectionManager>();

            if (characterSelectionManager)
                characterSelectionManager.SetPlayerReady();
            else
                Debug.LogError("characterSelectionManager: " + characterSelectionManager);
        });
    }

    private void Start()
    {
        Lobby lobby = LobbyManager.Instance.GetLobby();
        lobbyNameText.text = $"Lobby Name: {lobby?.Name}";
        lobbyCodeText.text = $"Lobby Code: {lobby?.LobbyCode}";
    }
}
