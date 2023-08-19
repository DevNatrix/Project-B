using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [HideInInspector] public static Recoil Instance;

    [Header("Recoil")]
    //Vector3 currentRotation;
    //Vector3 targetRotation;
    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    //[SerializeField] float snappiness;
    //[SerializeField] float returnSpeed;
	[SerializeField] Look look;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //RecoilVal();
    }

    public void RecoilVal()
    {
        //targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        //currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        
    }

    public void FireRecoil(float xMult = 1f, float yMult = 1f)
    {
		float newX = xMult * recoilX;
		float newY = yMult * recoilY;

		look.xRotation -= newX;
		look.yRotation += Random.Range(-newY, newY);

		//targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY));
    }
}
