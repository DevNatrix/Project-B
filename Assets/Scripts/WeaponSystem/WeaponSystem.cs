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

    public void BuyWeapon(GameObject WeaponType)
    {
        Instantiate(WeaponType, WeaponHolder.transform);
        Debug.Log($"Bought {WeaponType}");
    }
}
