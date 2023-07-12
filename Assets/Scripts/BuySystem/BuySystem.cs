using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuySystem : MonoBehaviour
{
    public static BuySystem Instance { get; private set; }
    PlayerControls playerControls;
    public GameObject BuyScreenGO;
    public GameObject WeaponHolder;
    bool BuyScreenVisual = false;
    public GameObject WeaponCam;
    public GameObject camDir;
    [HideInInspector] public GameObject WeaponIns;

    private void Awake()
    {
        playerControls = new PlayerControls();

        Instance = this;
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
        WeaponIns = Instantiate(WeaponType, WeaponHolder.transform);
        Debug.Log($"Bought {WeaponType}");
    }
}
