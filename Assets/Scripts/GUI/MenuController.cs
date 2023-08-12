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
	bool menu = false;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		playerControls = new PlayerControls();

		playerControls.UI.toggleMenu.performed += OnKeyPerformed;
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

	public void leaveServer()
	{
		client.sendTCPMessage("quit");
		SceneManager.LoadScene(0);
	}
}
