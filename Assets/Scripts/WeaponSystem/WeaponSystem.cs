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

    public int damage;

    public int maxDistance;

    public float fireRate;

    private float nextFire;

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
       if (playerControls.Weapon.Fire.IsPressed() && nextFire <= 0)
        {
            nextFire = 1 / fireRate;

            //Shoot Function
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
            }
        }
    }

    void Reload()
    {
        if(playerControls.Weapon.Reload.WasPressedThisFrame())
        {
            anim.Play("ReloadAnim");
            anim.SetBool("Reloading", true);
        }
    }

    public void SetReloadFalse()
    {
        //Reload Weapon
        anim.SetBool("Reloading", false);
    }
}
