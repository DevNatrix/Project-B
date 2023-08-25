using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [HideInInspector] public static Recoil Instance;

    [Header("Recoil")]
    Vector3 currentRotation;
    [SerializeField] float minRecoilX;
    [SerializeField] float maxRecoilX;
    [SerializeField] float recoilY;
    [SerializeField] float returnSpeed;
    public PlayerManager playerManager;
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
		currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		playerManager.xRotRecoil = currentRotation.x;
		playerManager.yRotRecoil = currentRotation.y;
	}

    public void FireRecoil()
    {
		snapBackTimer = timeBeforeSnapBack;
		currentRotation += new Vector3(Random.Range(-minRecoilX, -maxRecoilX), Random.Range(-recoilY, recoilY), 0);
    }
}
