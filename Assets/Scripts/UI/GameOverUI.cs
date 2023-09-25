using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text recipeDeliveredCountText;
    [SerializeField] private TMP_Text recipeFailedCountText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        replayButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();

            recipeDeliveredCountText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
            recipeFailedCountText.text = DeliveryManager.Instance.GetFailedRecipesAmont().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        replayButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}