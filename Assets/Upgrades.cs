using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	public static List<int> upgradeLevels;
	List<string> upgradeNames;

	public TextMeshProUGUI pointText;

	/*upgrades to do:
	 * health
	 * heal
	 * regen
	 * shield
	 * reflect damage
	 * damage
	 * charge shot
	 * headshot mult
	 * firerate
	 * pierce players
	 * wallhacks
	 * multiple bullets
	 * endless ammo 
	 * no recoil
	 * reload speed
	 * clip size
	 * instant reload
	 * speed
	 * dash
	 * slash
	 * double jump
	 * recharge on hit
	 * 
	 * multple upgrades on one (health, damage, speed)
	 */

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
		int upgradeID = upgradeNames.IndexOf(upgrade.title);

		upgradeLevels[upgradeID]++;
		unlockedUpgrades[upgradeID] = true;
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
		return unlockedUpgrades[upgradeNames.IndexOf(upgradeName)];
	}

	public bool isUpgradeUnlocked(int upgradeID)
	{
		return unlockedUpgrades[upgradeID];
	}

	public int getUpgradeLevel(string upgradeName)
	{
		return upgradeLevels[upgradeNames.IndexOf(upgradeName)];
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
