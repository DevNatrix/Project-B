using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
	public GameObject readyButton;
	public GameObject waitingMessage;
	public Transform upgradesParent;

	public static bool doneUpgrading;

	[SerializeField] ServerEvents serverEvents;
	[SerializeField] GameManager gameManager;

	public static Upgrades instance;

	List<UpgradeInfo> upgradeInfos;
	List<bool> unlockedUpgrades;
	List<int> upgradeLevels;
	List<string> upgradeNames;

	public int healthPerLevel = 50;
	public float damageMultPerLevel = .25f;
	public float speedMultPerLevel = .5f;
	public float headshotMultPerLevel = .1f;
	public float firerateMultPerLevel = .2f;
	public int regenPerUpgrade = 10;

	public TextMeshProUGUI pointText;

	private void Awake()
	{
		instance = this;
		resetUpgrades();
		changePoints(0);
	}

	public void finishedUpgrading()
	{
		readyButton.SetActive(false);
		waitingMessage.SetActive(true);
		doneUpgrading = true;

		serverEvents.sendEventToOtherClients("otherClientDoneUpgrading", new string[] {Client.ID + ""});
		gameManager.checkUpgradeStatus();
	}

	public void openUpgrades()
	{
		doneUpgrading = false;
		readyButton.SetActive(true);
		waitingMessage.SetActive(false);
		upgradesParent.localPosition = Vector3.zero;

		if(GameManager.points == 0)
		{
			finishedUpgrading();
		}
	}

	public void chooseUpgrade(UpgradeInfo upgrade)
	{
		int upgradeID = upgradeNames.IndexOf(upgrade.title);

		upgradeLevels[upgradeID]++;
		unlockedUpgrades[upgradeID] = true;

		if(GameManager.points == 0)
		{
			finishedUpgrading();
		}
	}

	public void resetUpgrades()
	{
		upgradeInfos = new List<UpgradeInfo>(FindObjectsOfType<UpgradeInfo>(true)); //get all info scripts

		unlockedUpgrades = new List<bool>(); //clear the lists
		upgradeNames = new List<string>();
		upgradeLevels = new List<int>();

		foreach (UpgradeInfo upgrade in upgradeInfos) //re-fill the lists
		{
			upgrade.resetUpgrade();
			unlockedUpgrades.Add(false);
			upgradeNames.Add(upgrade.title);
			upgradeLevels.Add(0);
		}
	}

	public bool isUpgradeUnlocked(string upgradeName)
	{
		int upgradeID = upgradeNames.IndexOf(upgradeName);
		if(upgradeID == -1)
		{
			Debug.LogError("Could not find upgrade " + upgradeName);
			return false;
		}
		return unlockedUpgrades[upgradeID];
	}

	public bool isUpgradeUnlocked(int upgradeID)
	{
		return unlockedUpgrades[upgradeID];
	}

	public int getUpgradeLevel(string upgradeName)
	{
		int upgradeID = upgradeNames.IndexOf(upgradeName);
		if (upgradeID == -1)
		{
			Debug.LogError("Could not find upgrade " + upgradeName);
			return 0;
		}
		return upgradeLevels[upgradeID];
	}

	public int getUpgradeLevel(int upgradeID)
	{
		return upgradeLevels[upgradeID];
	}

	public void changePoints(int points)
	{
		GameManager.points += points;

		pointText.text = "Points: " + GameManager.points;
	}
}
