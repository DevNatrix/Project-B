using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    PlayerControls playerControls;
    public GameObject BuyScreenGO;
    public GameObject WeaponHolder;
    bool BuyScreenVisual = false;

    [Header("References To Weapons")]
    public GameObject AK47;

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
        if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && !BuyScreenVisual)
        {
            BuyScreenGO.SetActive(true);
            BuyScreenVisual = true;
        }
        else if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && BuyScreenVisual)
        {
            BuyScreenGO.SetActive(false);
            BuyScreenVisual = false;
        }
    }

    public void BuyWeapon()
    {
        Instantiate(AK47, WeaponHolder.transform);
        Debug.Log("Bought Weapon");
    }
}
