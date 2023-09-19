using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private TMP_Text soundEffectsText;
    [SerializeField] private TMP_Text musicText;

    [Space]
    [Space]
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAltButton;
    [SerializeField] private Button gamepadPauseButton;

    [SerializeField] private TMP_Text moveUpText;
    [SerializeField] private TMP_Text moveDownText;
    [SerializeField] private TMP_Text moveRightText;
    [SerializeField] private TMP_Text moveLeftText;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private TMP_Text interactAltText;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text gamepadInteractText;
    [SerializeField] private TMP_Text gamepadInteractAltText;
    [SerializeField] private TMP_Text gamepadPauseText;

    [Space]
    [SerializeField] private Transform pressToRebindKeyTransform;

    private GameInput gameInput;
   
    private Action onCloseButtonAction;

    private void Awake()
    {
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseButtonAction();
        });

        moveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp); });
        moveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown); });
        moveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft); });
        moveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight); });
        interactButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        interactAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlternate); });
        pauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });
        gamepadInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact); });
        gamepadInteractAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_InteractAlternate); });
        gamepadPauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Pause); });
    }

    private void Start()
    {
        gameInput = FindObjectOfType<GameInput>();

        GameManager.Instance.OnGameUnpaused += GameManger_OnGameUnpaused;
        UpdateVisual();
        Hide();
        HidePressToRebindKey();
    }

    private void GameManger_OnGameUnpaused(object sender, System.EventArgs e) => Hide();

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = gameInput.GetBindingText(GameInput.Binding.MoveUp);
        moveDownText.text = gameInput.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = gameInput.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = gameInput.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = gameInput.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = gameInput.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseText.text = gameInput.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadInteractAltText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamepadPauseText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
        soundEffectsButton.Select();
    }
    private void Hide() { gameObject.SetActive(false); }

    private void ShowPressToRebindKey() => pressToRebindKeyTransform.gameObject.SetActive(true);
    private void HidePressToRebindKey() => pressToRebindKeyTransform.gameObject.SetActive(false);

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        gameInput.RebindBinding(binding,
            () =>
            {
                HidePressToRebindKey();
                UpdateVisual();
            });
    }
}
