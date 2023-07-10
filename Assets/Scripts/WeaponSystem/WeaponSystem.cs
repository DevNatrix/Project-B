using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    PlayerControls playerControls;
    public GameObject BuyScreenGO;
    bool ClickFrame = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        BuyScreen();
    }

    void BuyScreen()
    {
        if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && !ClickFrame)
        {
            BuyScreenGO.SetActive(true);
            ClickFrame = true;
        }
        else if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && ClickFrame)
        {
            BuyScreenGO.SetActive(false);
            ClickFrame = false;
        }
    }
}
