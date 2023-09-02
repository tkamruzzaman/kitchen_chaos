using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameInput gameInput;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    private bool isWalking;
    [SerializeField]
    public bool IsWalking { get => isWalking; private set { isWalking = value; } }

    private void Awake()
    {
        gameInput = FindObjectOfType<GameInput>();
    }

    private void Update()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        IsWalking = moveDirection != Vector3.zero;

        transform.position += moveSpeed * Time.deltaTime * moveDirection ;

        transform.forward = Vector3.Slerp(transform.forward,  moveDirection, Time.deltaTime * rotationSpeed);

    }

}
