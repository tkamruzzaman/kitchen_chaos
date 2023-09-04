using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    [Range(0,5)]
    public int cuttingProgessMax;
}
