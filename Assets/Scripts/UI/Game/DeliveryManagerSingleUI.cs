using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text recipeNameText;

    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName + "  $"+ recipeSO.recipeScore;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) { continue; }

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}