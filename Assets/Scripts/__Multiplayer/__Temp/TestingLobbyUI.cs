using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(() =>
        {
            MultiplayerGameManager.Instance.StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectionScene);
        });

        joinGameButton.onClick.AddListener(() =>
        {
            MultiplayerGameManager.Instance.StartClient();
        });
    }
}
