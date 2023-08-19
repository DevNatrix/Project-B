using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [HideInInspector] public static Recoil Instance;

    [Header("Recoil")]
    Vector3 currentRotation;
    Vector3 targetRotation;
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;
    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;
    [SerializeField] float returnSpeedSlow;
    public Look look;
	public float timeBeforeSnapBack = .4f;
	public float snapBackTimer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
		snapBackTimer -= Time.deltaTime;
		if(snapBackTimer < 0f)
		{
			currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		}
		else
		{
			currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeedSlow * Time.deltaTime);
		}
		look.xRotRecoil = currentRotation.x;
		look.yRotRecoil = currentRotation.y;
	}

    public void FireRecoil()
    {
		snapBackTimer = timeBeforeSnapBack;
		targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
