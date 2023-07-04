using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] Movement movement;
    //[SerializeField] MenuManager menuManager;
    [SerializeField] Look look;

    public PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovementActions;
    PlayerControls.UIActions uiAtions;

    Vector2 horizontalInput;
    Vector2 mouseInput;

    private void Awake()
    {
        controls = new PlayerControls();
        groundMovementActions = controls.GroundMovement;
        uiAtions = controls.UI;

        groundMovementActions.Horizontal.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

        groundMovementActions.Jump.performed += _ => movement.jump = true;
        groundMovementActions.Jump.performed += _ => movement.jumping = true;
        groundMovementActions.Jump.canceled += _ => movement.jumping = false;
        groundMovementActions.Slide.performed += _ => movement.slidingRequested = true;
        groundMovementActions.Slide.canceled += _ => movement.slidingRequested = false;
        groundMovementActions.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        groundMovementActions.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
		groundMovementActions.Dash.performed += _ => StartCoroutine(movement.dash());
        //uiAtions.toggleMenu.performed += _ => menuManager.toggleMenu(!menuManager.menuIsActive);
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDestroy()
    {
        controls.Disable();
    }

    private void Update()
    {
        movement.ReceiveInput(horizontalInput);
        look.ReceiveInput(mouseInput);
    }
}
