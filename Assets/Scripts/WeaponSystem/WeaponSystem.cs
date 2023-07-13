using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("References")]
    private GameObject cam;
    public WeaponInfo weaponInfo;
    public Animator anim;
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

    private void Awake()
    {
        playerControls = new PlayerControls();

        cam = GameReferences.Instance.MainCam;

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
                if (hit.transform.gameObject.GetComponent<OtherClient>())
                {
                    hit.transform.gameObject.GetComponent<OtherClient>().TakeDamage(damage);
                    string ClientID =  hit.transform.gameObject.GetComponent<OtherClient>().ID.ToString();
                    serverEvents.sendEvent("Damage", new string[]{damage.ToString(), ClientID});
                }

                currentAmmo--;
            }
        }
    }

    void Reload()
    {
        if(playerControls.Weapon.Reload.WasPressedThisFrame())
        {
            if (currentAmmo < maxAmmo && AmmoInReserve > 0)
            {
                int ammoToAdd = maxAmmo - currentAmmo;
                int clamped = Mathf.Min(ammoToAdd, AmmoInReserve);
                AmmoInReserve -= clamped;
                currentAmmo += clamped;

                //Play Reload Animation
                anim.SetBool("Reloading", true);
            }
        }
    }

    //Stops reloading
    public void SetReloadFalse()
    {
        anim.SetBool("Reloading", false);
    }
}
