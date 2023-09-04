using System;
using System.Collections;
using System.Collections.Generic;
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
        }
    }

    public override void InteractAlternate(Player player)
    {

    }
}