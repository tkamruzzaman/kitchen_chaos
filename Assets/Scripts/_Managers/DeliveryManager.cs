using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler<OnRecipeSuccessEventArgs> OnRecipeSuccess;
    public class OnRecipeSuccessEventArgs: EventArgs
    {
       public DeliveryCounter deliveryCounter;
    }
    public event EventHandler<OnRecipeFailedEventArgs> OnRecipeFailed;
    public class OnRecipeFailedEventArgs: EventArgs
    {
        public DeliveryCounter deliveryCounter;
    }

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();

    private float spawnRecipeTimer = 4f;  //FIX_ME 0 is the value
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

    private int successfulRecipesAmount = 0;
    private int failedRecipesAmount = 0;
    private int totalEarnedMoney = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsServer) { return; }

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int watingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[watingRecipeSOIndex];

                SpawnNewWaitingRecipeClientRpc(watingRecipeSOIndex);           
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int watingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[watingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(DeliveryCounter deliveryCounter, PlateKitchenObject plateKitchenObject)
    {
        NetworkObject deliveryCounterNetworkObject = null;

        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    //Cycling through all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //Cycling through all ingredients in the plate
                        if(plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            //Ingredients does matchs!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //the recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    //Player deliver the correct recipe!
                    deliveryCounterNetworkObject = deliveryCounter.GetNetworkObject();
                    int waitingRecipeScore = waitingRecipeSO.recipeScore;

                    DeliverCorrectRecipeServerRpc(i, waitingRecipeScore, deliveryCounterNetworkObject);
                    return;
                }
            }
        }
        //No matches found!
        //Player did not deliver a correct recipe
        deliveryCounterNetworkObject = deliveryCounter.GetNetworkObject();
        DeliverIncorrectRecipeServerRpc(deliveryCounterNetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(
        int waitingRecipeSOListIndex, 
        int waitingRecipeScore, 
        NetworkObjectReference deliveryCounterReference)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex, 
            waitingRecipeScore, 
            deliveryCounterReference);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(
        int waitingRecipeSOListIndex, 
        int waitingRecipeScore, 
        NetworkObjectReference deliveryCounterReference)
    {
        if (deliveryCounterReference.TryGet(out NetworkObject deliveryCounterNetworkObject))
        {
            DeliveryCounter deliveryCounter = deliveryCounterNetworkObject.GetComponent<DeliveryCounter>();

            successfulRecipesAmount++;
            totalEarnedMoney += waitingRecipeScore;
            waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
            OnRecipeSuccess?.Invoke(this, new OnRecipeSuccessEventArgs
            {
                deliveryCounter = deliveryCounter
            });
        }
        else
        {
            Debug.LogError("DeliveryCounterReference not found on server, likely because it already has been destroyed/despawned");
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc(NetworkObjectReference deliveryCounterReference)
    {
        DeliverIncorrectRecipeClientRpc(deliveryCounterReference);
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc(NetworkObjectReference deliveryCounterReference)
    {
        if (deliveryCounterReference.TryGet(out NetworkObject deliveryCounterNetworkObject))
        {
            DeliveryCounter deliveryCounter = deliveryCounterNetworkObject.GetComponent<DeliveryCounter>();

            failedRecipesAmount++;

            OnRecipeFailed?.Invoke(this, new OnRecipeFailedEventArgs
            {
                deliveryCounter = deliveryCounter
            });
        }
        else
        {
            Debug.LogError("DeliveryCounterReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList() => waitingRecipeSOList;

    public int GetSuccessfulRecipesAmount() => successfulRecipesAmount;

    public int GetFailedRecipesAmont() => failedRecipesAmount;

    public int GetTotalIncomeAmount() => totalEarnedMoney;
}
