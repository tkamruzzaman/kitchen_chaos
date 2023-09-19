using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    [Space]
    [SerializeField] private OptionsUI optionsUI;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        optionsButton.onClick.AddListener(() =>
        {
            optionsUI.Show();
        });
        
        mainMenuButton.onClick.AddListener(() => 
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        Hide();
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e) => Show();

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e) => Hide();

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);
}
