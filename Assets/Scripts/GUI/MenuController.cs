using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	PlayerControls playerControls;
	public GameObject startMenu;
	public GameObject menuParent;
	public List<GameObject> subMenus;
	public Client client;
	[HideInInspector] public static bool menu = true;
	[HideInInspector] public static bool typing = false;
	public PlayerManager playerManager;

	public Camera spectateCam;
	public Camera gunCam;
	public GameObject InGameGUI;
	public GameObject TeamSelection;

	public GameObject deathScreen;
	public AbilityManager abilityManager;

	public void triggerDeathMenu()
	{
		setSpectate(true);
		deathScreen.SetActive(true);
		menu = true;
		Cursor.lockState = CursorLockMode.None;
		abilityManager.chooseSelectable();
	}

	public void closeDeathScreen()
	{
		setSpectate(false);
		deathScreen.SetActive(false);
		menu = false;
		Cursor.lockState = CursorLockMode.Locked;
		playerManager.respawn();
	}


	private void Awake()
	{
		deathScreen.SetActive(false);
		menuParent.SetActive(true);
		setSpectate(true);

		Cursor.lockState = CursorLockMode.None;
		playerControls = new PlayerControls();

		playerControls.UI.toggleMenu.performed += OnKeyPerformed;
	}

    private void Start()
    {
		TeamSelection.SetActive(true);
	}

    private void OnEnable()
	{
		playerControls.Enable();
	}

	private void OnDisable()
	{
		playerControls.Disable();
	}

	private void OnKeyPerformed(InputAction.CallbackContext context)
	{
		toggleMenu();
	}
	private void KMS(InputAction.CallbackContext context)
	{
		triggerDeathMenu();
	}

	public void toggleMenu()
	{
		menu = !menu;
		if (menu)
		{
			Cursor.lockState = CursorLockMode.None;
			menuParent.SetActive(true);
			foreach (GameObject subMenu in subMenus)
			{
				subMenu.SetActive(false);
			}
			startMenu.SetActive(true);
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			menuParent.SetActive(false);
		}
	}

	//button events
	public void enableGameObject(GameObject objectToEnable)
	{
		objectToEnable.SetActive(true);
	}
	public void disableGameObject(GameObject objectToDisable)
	{
		objectToDisable.SetActive(false);
	}

	public void setTeam(int team)
	{
		playerManager.setTeam(team);

		setSpectate(false);
		toggleMenu();
	}

	public void setSpectate(bool isEnabled)
	{
		gunCam.enabled = !isEnabled;
		InGameGUI.SetActive(!isEnabled);
		client.showClient = !isEnabled;

		spectateCam.enabled = isEnabled;
	}

	public void leaveServer()
	{
		try
		{
			client.sendTCPMessage("quit");
		}
		catch
		{
			Debug.Log("Couldn't send leave message");
		}
		SceneManager.LoadScene(0);
	}

	public void setVolume(float volume)
	{
		Debug.Log(volume);
		AudioPlayer.volumeMult = volume;
	}
}
