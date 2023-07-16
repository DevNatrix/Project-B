using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    PlayerControls playerControls;
    private int selectedWeapon = 0;
    public bool switchonScroll = true;
    private Animator anim;

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

    void Start()
    {
        SelectWeapon();
    }


    void Update()
    {
        int previousWeapon = selectedWeapon;

        if(playerControls.Weapon.PrimaryWeapon.WasPressedThisFrame())
        {
            selectedWeapon = 0;
        }

        if (playerControls.Weapon.SecondaryWeapon.WasPressedThisFrame())
        {
            selectedWeapon = 1;
        }

        if (playerControls.Weapon.Knife.WasPressedThisFrame())
        {
            selectedWeapon = 2;
        }

        SwitchOnScrollWheel();

        if (previousWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SwitchOnScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon += 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon -= 1;
            }
        }
    }

    void SelectWeapon()
    {
        if(selectedWeapon >= transform.childCount)
        {
            selectedWeapon = transform.childCount - 1;
        }

        int i = 0;

        foreach (Transform _weapon in transform)
        {
            if(i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);
            }
            else
            {
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}