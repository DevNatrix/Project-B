using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuySystem : MonoBehaviour
{
    public static BuySystem Instance { get; private set; }
    PlayerControls playerControls;
    public GameObject BuyScreenGO;
    bool BuyScreenVisual = false;
    [HideInInspector] public GameObject WeaponIns;
    [HideInInspector] public GameObject WeaponType;

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
			Cursor.lockState = CursorLockMode.None;
		}
        else if(playerControls.Weapon.BuyScreen.WasPressedThisFrame() && BuyScreenVisual)
        {
			Cursor.lockState = CursorLockMode.Locked;
            BuyScreenGO.SetActive(false);
            BuyScreenVisual = false;
        }
    }

    public void BuyWeapon(GameObject _WeaponType)
    {
        WeaponType = _WeaponType;
        WeaponIns = Instantiate(WeaponType, GameReferences.Instance.weaponHolder.transform);
        Debug.Log($"Bought {WeaponType.name}");
        WeaponSwitcher.Instance.AddItem(_WeaponType, _WeaponType.GetComponent<WeaponSystem>().weaponType);
    }
}
