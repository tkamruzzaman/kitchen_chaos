using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int plateSpawnedAmount;
    private int plateSpawnedAmountMax = 4;

    private void Update()
    {
        if(!IsServer) { return; }

        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;

            //KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, this);
            if (GameManager.Instance.IsGamePlaying() && plateSpawnedAmount < plateSpawnedAmountMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        plateSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is empty handed
            if (plateSpawnedAmount > 0)
            {
                //there is al least one plate here
                
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                RemovePlateServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlateServerRpc()
    {
        RemovePlateClientRpc();
    }

    [ClientRpc]
    private void RemovePlateClientRpc()
    {
        plateSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }

    public override void InteractAlternate(Player player)
    {
    }
}