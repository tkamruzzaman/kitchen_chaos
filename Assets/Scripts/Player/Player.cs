using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomthing;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Vector3 spawnPosition; 
    [SerializeField] private Vector3 spawnRotation;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    public bool IsWalking { get => isWalking; private set { isWalking = value; } }

    private Vector3 lastInteractDirection;

    private BaseCounter selectedCounter;

    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Debug.LogError("There is more than one Player instance"); }
    }

    private void Start()
    {
        gameInput = FindObjectOfType<GameInput>();

        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

        transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(spawnRotation));
    }

    private void OnDestroy()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) { return; }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) { return; }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver()) { return; }

        HandleMovement();
        HandleInterection();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;

        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius,
            moveDirection, Quaternion.identity, moveDistance, countersLayerMask);

        isWalking = moveDirection != Vector3.zero;

        //testing can we move on one direction in diagonal movement
        if (!canMove)
        {
            //can not move towards moveDirection
            //attempt only X move
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < -0.5f || moveDirection.x > +0.5f)
                && !Physics.BoxCast(transform.position, Vector3.one * playerRadius,
                 moveDirX, Quaternion.identity, moveDistance, countersLayerMask);

            if (canMove)
            {
                //can move only in X
                moveDirection = moveDirX;
            }
            else
            {
                //cannot move only on X
                //attampt only Z move
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = (moveDirection.z < -0.5f || moveDirection.z > +0.5f)
                    && !Physics.BoxCast(transform.position, Vector3.one * playerRadius,
                     moveDirZ, Quaternion.identity, moveDistance, countersLayerMask);

                if (canMove)
                {
                    //can only move in Z
                    moveDirection = moveDirZ;
                }
                else
                {
                    //Can not move at all
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDistance * moveDirection;
        }

        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }

    private void HandleInterection()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDirection != Vector3.zero)
        {
            lastInteractDirection = moveDirection;
        }

        float interectDistance = 2f;

        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interectDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomthing?.Invoke(this, EventArgs.Empty);
        }
    }

    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void ClearKitchenObject() => kitchenObject = null;

    public bool HasKitchenObject() => kitchenObject != null;
}
