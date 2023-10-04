using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
 
    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinWithId(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
