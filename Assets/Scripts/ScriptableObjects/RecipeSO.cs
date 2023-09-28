using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;

    public int recipeScore;
    public string recipeName;

    [ContextMenu(nameof(SetScore))]
    private void SetScore()
    {
        int scoreMultiplier = 5;
        recipeScore = scoreMultiplier * kitchenObjectSOList.Count;
    }
}
