using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	PlayerControls playerControls;
	public GameObject startMenu;
	public GameObject menuParent;
	public List<GameObject> subMenus;
	public Client client;
	public PlayerManager playerManager;

	public Camera spectateCam;
	public GameObject InGameGUI;

	public GameObject deathScreen;
	public GameObject lobbyScreen;
	public GameObject lobbyStartButton;
	public GameObject waitMessage;
	public TextMeshProUGUI matchTimerText;
	public AbilityManager abilityManager;


	public GameObject buyScreenObject;
	public ServerEvents serverEvents;

	public Image team0ButtonImage;
	public Image team1ButtonImage;

	public Transform enemyScoreboardParent;
	public Transform friendlyScoreboardParent;
	public TextMeshProUGUI matchText;
	public TextMeshProUGUI roundText;
	public GameObject matchStartParent;

	public GameObject upgradeMenu;
	public Upgrades upgrades;
	
	[HideInInspector] public static bool buyMenu = false;
	[HideInInspector] public static bool mainMenu = false;
	[HideInInspector] public static bool lobby = false;
	[HideInInspector] public static bool upgrading = false;
	[HideInInspector] public static bool countdown = false;
	[HideInInspector] public static bool typing = false;

	//locks
	[HideInInspector] public static bool cursorUnlocked;
	[HideInInspector] public static bool lookLocked;
	[HideInInspector] public static bool movementLocked;
	[HideInInspector] public static bool weaponUseLocked;

	private void Update()
	{
		cursorUnlocked = buyMenu || mainMenu || lobby || upgrading;
		lookLocked = buyMenu || mainMenu || lobby || GameManager.dead || upgrading;
		movementLocked = typing || countdown;
		weaponUseLocked = buyMenu || mainMenu || lobby || upgrading || countdown || GameManager.dead || typing;

		if (cursorUnlocked)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		lobbyStartButton.SetActive(Client.owner);
		waitMessage.SetActive(!Client.owner);

		if (GameManager.matchTimer > 0)
		{
			matchStartParent.SetActive(true);
			matchTimerText.text = Mathf.Round(GameManager.matchTimer * 10) / 10 + "";

			GameManager.matchTimer -= Time.deltaTime;
			countdown = true;

			if(PlayerManager.team == 0)
			{
				matchText.text = "Matches: " + GameManager.team0MatchCount + " - " + GameManager.team1MatchCount;
				roundText.text = "Rounds: " + GameManager.team0RoundCount + " - " + GameManager.team1RoundCount;
			}
			else
			{
				matchText.text = "Matches: " + GameManager.team1MatchCount + " - " + GameManager.team0MatchCount;
				roundText.text = "Rounds: " + GameManager.team1RoundCount + " - " + GameManager.team0RoundCount;
			}
		}
		else
		{
			countdown = false;
			matchStartParent.SetActive(false);
		}
	}

	public void setUpgradeMenu(bool enable)
	{
		upgrading = enable;
		upgradeMenu.SetActive(enable);
		if(enable )
		{
			upgrades.openUpgrades();
		}
	}

	public void setLobbyMenu(bool enable)
	{
		setSpectate(enable);
		lobby = enable;
		lobbyScreen.SetActive(enable);
	}

	public void sendStartGameEvent()
	{
		serverEvents.sendGlobalEvent("startGame", new string[] { GameManager.rounds + ""});	

		setLobbyMenu(false);
		
		//put some checks to make sure that there is 2 or more players
		//also check for team balance
	}

	public void death()
	{
		setSpectate(true);
		GameManager.dead = true;
	}
	public void spawn()
	{
		setSpectate(false);
		GameManager.dead = false;
	}

	private void Awake()
	{
		deathScreen.SetActive(false);
		menuParent.SetActive(false);
		upgradeMenu.SetActive(false);
		setLobbyMenu(true);

		playerControls = new PlayerControls();

		playerControls.UI.toggleMenu.performed += mainMenuKey;
		playerControls.Weapon.BuyScreen.performed += buyMenuKey;
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
		if(!typing && !mainMenu)
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

		if(team == 0)
		{
			team0ButtonImage.color = Color.white;
			team1ButtonImage.color = Color.gray;
		}
		else
		{
			team1ButtonImage.color = Color.white;
			team0ButtonImage.color = Color.gray;
		}
	}

	public void setSpectate(bool isEnabled)
	{
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
