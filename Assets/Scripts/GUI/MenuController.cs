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
	[HideInInspector] public static bool menu;
	[HideInInspector] public static bool typing = false;
	public PlayerManager playerManager;

	public Camera spectateCam;
	public Camera gunCam;
	public GameObject InGameGUI;
	public GameObject TeamSelection;

	public GameObject deathScreen;
	public AbilityManager abilityManager;

	public bool buyMenu = false;
	public bool deathMenu = false;
	public bool mainMenu = true;

	public GameObject buyScreenObject;


	private void Update()
	{
		menu = buyMenu || deathMenu || mainMenu;

		if (menu)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	public void triggerDeathMenu()
	{
		abilityManager.chooseSelectable();
		setSpectate(true);
		deathScreen.SetActive(true);
		deathMenu = true;
	}

	public void closeDeathScreen()
	{
		setSpectate(false);
		deathScreen.SetActive(false);
		deathMenu = false;
		playerManager.respawn();
	}

	private void Awake()
	{
		deathScreen.SetActive(false);
		menuParent.SetActive(true);
		setSpectate(true);

		Cursor.lockState = CursorLockMode.None;
		playerControls = new PlayerControls();

		playerControls.UI.toggleMenu.performed += mainMenuKey;
		playerControls.Weapon.BuyScreen.performed += buyMenuKey;
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

	private void mainMenuKey(InputAction.CallbackContext context)
	{
		toggleMainMenu();
	}
	
	private void buyMenuKey(InputAction.CallbackContext context)
	{
		if(!typing && !mainMenu && !deathMenu)
		{
			buyScreenObject.SetActive(!buyScreenObject.activeSelf);
			buyMenu = buyScreenObject.activeSelf;
		}
	}

	public void toggleMainMenu()
	{
		mainMenu = !mainMenu;
		if (mainMenu)
		{
			menuParent.SetActive(true);
			foreach (GameObject subMenu in subMenus)
			{
				subMenu.SetActive(false);
			}
			startMenu.SetActive(true);
		}
		else
		{
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
		toggleMainMenu();
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
