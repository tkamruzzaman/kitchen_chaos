using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnInteracted;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is not carrying anytging
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnInteracted?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //player is carrying kitchenobject
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //player is holding a Plate
                if (plateKitchenObject.TryAddIngredient(kitchenObjectSO))
                {
                    //placing valid kitchenObjectSO item on the plate
                    OnInteracted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
    }
}