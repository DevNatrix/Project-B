using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    [HideInInspector] public static WeaponSystem Instance;
    PlayerControls playerControls;

    [Header("References")]
    private GameObject cam;
    public Animator anim;
    public GameObject groundPrefab;
    public GameObject bulletHole;
    [HideInInspector] public ServerEvents serverEvents;

    [Header("Info")]
    public string WeaponID;

    public int damage;

    public int maxDistance;

    public float fireRate;

    private float nextFire;

    [Header("Ammo")]
    public int AmmoInReserve;
    public int currentAmmo;
    public int maxAmmo;

    [Header("UI")]
    private TextMeshProUGUI AmmoAndMagText;

    [Header("Other")]
    public float dropForce = 0.5f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        Instance = this;

        cam = GameReferences.Instance.MainCam;
        AmmoAndMagText = GameReferences.Instance.AmmoAndMagText;

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
		serverEvents = GameObject.Find("game manager").GetComponent<ServerEvents>();
    }

    void Update()
    {
        Shoot();
        Reload();
        Debug.DrawRay(cam.transform.position, cam.transform.forward);

        if(nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }


        AmmoAndMagText.text = currentAmmo.ToString() + "/" + AmmoInReserve.ToString();

        if(playerControls.Weapon.Drop.WasPerformedThisFrame())
        {
            DropItem();
        }
    }

    private void Shoot()
    {
       if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0 && currentAmmo > 0 && gameObject.GetComponent<Animator>().GetBool("Reloading") == false)
        {
            nextFire = 1 / fireRate;

            //Shoot raycast
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance))
            {
                if (hit.transform.gameObject.GetComponent<Health>())
                {
                    hit.transform.gameObject.GetComponent<Health>().TakeDamage(damage);
                    string ClientID = hit.transform.gameObject.GetComponent<OtherClient>().ID.ToString();
                    serverEvents.sendEvent("Damage", new string[] { damage.ToString(), ClientID });
                }

                Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
            }

            currentAmmo--;
        }
    }

    void Reload()
    {
        if(playerControls.Weapon.Reload.WasPressedThisFrame())
        {
            if (currentAmmo < maxAmmo && AmmoInReserve > 0)
            {
                //Play Reload Animation
                anim.SetBool("Reloading", true);
            }
        }
    }

    public void ReloadAmmo()
    {
        int ammoToAdd = maxAmmo - currentAmmo;
        int clamped = Mathf.Min(ammoToAdd, AmmoInReserve);
        AmmoInReserve -= clamped;
        currentAmmo += clamped;
    }

    //Stops reloading
    public void SetReloadFalse()
    {
        anim.SetBool("Reloading", false);
    }

    public void DropItem()
    {
        //Store gun info on the groundPrefab
        PickUpSystem.AmmoInReserve = AmmoInReserve;
        PickUpSystem.currentAmmo = currentAmmo;
        PickUpSystem.maxAmmo = maxAmmo;

        Destroy(gameObject);
        Instantiate(groundPrefab, cam.transform.position + (cam.transform.forward * dropForce), Quaternion.identity);
        WeaponSwitcher.Instance.UnequipWeapons();
    }
}
