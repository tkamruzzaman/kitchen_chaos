using System;

public class TrashCounter : BaseCounter
{
    public event EventHandler<OnAnyObjectTrashedEventArgs> OnAnyObjectTrashed;
    public class OnAnyObjectTrashedEventArgs: EventArgs
    {
        public KitchenObject kitchenObject;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject kitchenObject = player.GetKitchenObject();
            kitchenObject.DestroySelf();

            OnAnyObjectTrashed?.Invoke(this, new OnAnyObjectTrashedEventArgs
            {
                kitchenObject = kitchenObject
            }); 
        }
    }

    public override void InteractAlternate(Player player)
    {
    }
}
