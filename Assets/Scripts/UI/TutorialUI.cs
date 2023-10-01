using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TMP_Text keyMoveUpText;
    [SerializeField] private TMP_Text keyMoveDownText;
    [SerializeField] private TMP_Text keyMoveLeftText;
    [SerializeField] private TMP_Text keyMoveRightText;
    [SerializeField] private TMP_Text keyInteractText;
    [SerializeField] private TMP_Text keyInteractAlternateText;
    [SerializeField] private TMP_Text keyPauseText;
    [SerializeField] private TMP_Text keyGamepadInteractText;
    [SerializeField] private TMP_Text keyGamepadInteractAlternateText;
    [SerializeField] private TMP_Text keyGamepadPauseText;
    
    private GameInput gameInput;

    private void Start()
    {
        gameInput = FindObjectOfType<GameInput>();
        gameInput.OnBindingRebind += GameInput_OnBindingRebind;

        GameManager.Instance.OnLocalPlayerReady += GameManager_OnLocalPlayerReady;
        
        UpdateVisual();

        Show();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLocalPlayerReady -= GameManager_OnLocalPlayerReady;
    }

    private void GameManager_OnLocalPlayerReady(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }


    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = gameInput.GetBindingText(GameInput.Binding.MoveUp);
        keyMoveDownText.text = gameInput.GetBindingText(GameInput.Binding.MoveDown);
        keyMoveLeftText.text = gameInput.GetBindingText(GameInput.Binding.MoveLeft);
        keyMoveRightText.text = gameInput.GetBindingText(GameInput.Binding.MoveRight);
        keyInteractText.text = gameInput.GetBindingText(GameInput.Binding.Interact);
        keyInteractAlternateText.text = gameInput.GetBindingText(GameInput.Binding.InteractAlternate);
        keyPauseText.text = gameInput.GetBindingText(GameInput.Binding.Pause);
        keyGamepadInteractText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_Interact);
        keyGamepadInteractAlternateText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        keyGamepadPauseText.text = gameInput.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
