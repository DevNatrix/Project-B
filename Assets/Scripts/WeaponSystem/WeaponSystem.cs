using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("References")]
    private GameObject cam;
    public Animator anim;
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
    private GameObject AmmoDisplayGOS;

    private void Awake()
    {
        playerControls = new PlayerControls();

        cam = GameReferences.Instance.MainCam;
        AmmoAndMagText = GameReferences.Instance.AmmoAndMagText;
        AmmoDisplayGOS = GameReferences.Instance.AmmoDisplayGO;

    }

    private void OnEnable()
    {
        playerControls.Enable();
        AmmoDisplayGOS.SetActive(true);
    }

    private void OnDisable()
    {
        playerControls.Disable();
        AmmoDisplayGOS.SetActive(false);
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


        AmmoAndMagText.text = currentAmmo.ToString() + "/" + AmmoInReserve.ToString();
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
                Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));

            }
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
}
