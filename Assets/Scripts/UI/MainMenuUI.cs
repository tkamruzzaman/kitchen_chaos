using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        playButton.onClick.AddListener(PlayButtonAction);
        quitButton.onClick.AddListener(QuitButtonAction);
    }


    private void PlayButtonAction()
    {
        Loader.LoadScene(Loader.Scene.LobbyScene);
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
