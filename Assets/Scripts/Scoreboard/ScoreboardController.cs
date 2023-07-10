using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreboardController : MonoBehaviour
{
    PlayerControls playerControls;
    public GameObject ScoreBoardGO;
    private bool isKeyHeld = false;

    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.UI.Scoreboard.performed += OnKeyPerformed;
        playerControls.UI.Scoreboard.canceled += OnKeyCanceled;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnKeyPerformed(InputAction.CallbackContext context)
    {
        isKeyHeld = true;
        ScoreBoardGO.SetActive(true);
    }

    private void OnKeyCanceled(InputAction.CallbackContext context)
    {
        isKeyHeld = false;
        ScoreBoardGO.SetActive(false);
    }
}
