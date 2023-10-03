using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(() => 
        {
            FindObjectOfType<CharacterSelectionManager>().SetPlayerReady();
        });
    }
}
