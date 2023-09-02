using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private Vector2 inputVector = new Vector2(0, 0);

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        /* 
         * **** Old Input System   
        if (Input.GetKey(KeyCode.W)) { inputVector.y = +1; }
        if (Input.GetKey(KeyCode.S)) { inputVector.y = -1; }
        if (Input.GetKey(KeyCode.A)) { inputVector.x = -1; }
        if (Input.GetKey(KeyCode.D)) { inputVector.x = +1; }
        */

        inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}