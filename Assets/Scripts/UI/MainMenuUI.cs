using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayerPlayButton;
    [SerializeField] private Button multiplayerPlayButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        singleplayerPlayButton.onClick.AddListener(() => 
        {
            MultiplayerGameManager.isToPlaySingleplayer = true;

            Loader.LoadScene(Loader.Scene.LobbyScene);
        });

        multiplayerPlayButton.onClick.AddListener(() => 
        {
            MultiplayerGameManager.isToPlaySingleplayer = false;
            
            Loader.LoadScene(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(QuitButtonAction);
    }

    private void QuitButtonAction()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
