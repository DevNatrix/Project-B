using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
	public static List<bool> unlockedUpgrades;
	List<string> upgradeNames;

	private void Awake()
	{
		instance = this;
		resetUpgrades();
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
	}

	public void chooseUpgrade(UpgradeInfo upgrade)
	{
		Debug.Log("Got " + upgrade.title);
	}

	public void resetUpgrades()
	{
		upgradeInfos = new List<UpgradeInfo>(FindObjectsOfType<UpgradeInfo>(true)); //get all info scripts

		unlockedUpgrades = new List<bool>(); //clear the lists
		upgradeNames = new List<string>();

		foreach (UpgradeInfo upgrade in upgradeInfos) //re-fill the lists
		{
			upgrade.resetUpgrade();
			unlockedUpgrades.Add(false);
			upgradeNames.Add(upgrade.title);
		}
	}

	public bool isUpgradeUnlocked(string upgradeName)
	{
		return isUpgradeUnlocked(upgradeNames.IndexOf(upgradeName));
	}

	public bool isUpgradeUnlocked(int upgradeID)
	{
		return unlockedUpgrades[upgradeID];
	}
}
