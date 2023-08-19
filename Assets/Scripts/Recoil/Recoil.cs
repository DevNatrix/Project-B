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
    public Look look;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        RecoilVal();
    }

    public void RecoilVal()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        look.xRotRecoil = currentRotation.x;
        look.yRotRecoil = currentRotation.y;
    }

    public void FireRecoil()
    {
		targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
