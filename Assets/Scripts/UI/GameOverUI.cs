using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text recipeDeliveredCountText;
    [SerializeField] private TMP_Text recipeFailedCountText;
    [SerializeField] private TMP_Text totalIncomeAmountText;
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

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();

            recipeDeliveredCountText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
            recipeFailedCountText.text = DeliveryManager.Instance.GetFailedRecipesAmont().ToString();
            totalIncomeAmountText.text = "$" + DeliveryManager.Instance.GetTotalIncomeAmount().ToString();
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