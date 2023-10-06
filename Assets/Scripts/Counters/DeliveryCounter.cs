using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //Only accepts Plates
                DeliveryManager.Instance.DeliverRecipe(this, plateKitchenObject);

                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
    }
}
