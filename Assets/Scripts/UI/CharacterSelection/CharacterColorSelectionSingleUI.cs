using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectionSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image colorImage;
    [SerializeField] private Image selectedImage;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerGameManager.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerGameManager_OnPlayerDataNetworkListChanged;

        colorImage.color = MultiplayerGameManager.Instance.GetPlayerColor(colorId);

        UpdateSelectedImage();
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedImage();
    }

    private void UpdateSelectedImage()
    {
        if (MultiplayerGameManager.Instance.GetPlayerData().colorId == colorId)
        {
            selectedImage.gameObject.SetActive(true);
        }
        else
        {
            selectedImage.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged -= MultiplayerGameManager_OnPlayerDataNetworkListChanged;
    }

}
