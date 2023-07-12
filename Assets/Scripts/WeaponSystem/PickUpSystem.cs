using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    PlayerControls playerControls;

    [HideInInspector] public Transform cam;
    [HideInInspector] public GameObject eyesDirection;
    [SerializeField] public GameObject WeaponModel;
    [SerializeField] private WeaponInfo weaponInfo;

    void Awake()
    {
        playerControls = new PlayerControls();

        cam = BuySystem.Instance.WeaponCam.transform;
        eyesDirection = BuySystem.Instance.camDir;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        PickUpWeapon();
    }

    private void PickUpWeapon()
    {
        if (playerControls.Weapon.Interact.WasPressedThisFrame() && Physics.Raycast(cam.position, eyesDirection.transform.forward, out RaycastHit hitInfo, weaponInfo.maxDistance))
        {
            if (hitInfo.transform.name == WeaponModel.transform.name + "(Clone)")
            {
                Destroy(WeaponSystem.Instance.WeaponModelClone);
                BuySystem.Instance.WeaponIns.SetActive(true);
                Debug.Log("Does work");
            }
            Debug.Log(WeaponModel.transform.name + "(Clone)");
        }
    }
}
