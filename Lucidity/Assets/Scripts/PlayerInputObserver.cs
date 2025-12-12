using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputObserver : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInputRef;

    public Vector2 movement { get; private set; } = Vector2.zero;
    public Vector2 cameraMovement { get; private set; } = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        movement = new Vector2(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y));

        Debug.Log("Movement: " + movement);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraMovement = context.ReadValue<Vector2>();

        Debug.Log("Camera: " + cameraMovement);
    }
}
