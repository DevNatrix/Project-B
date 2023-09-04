using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
	public GameObject readyButton;
	public GameObject waitingMessage;

	public static bool doneUpgrading;

	[SerializeField] ServerEvents serverEvents;
	[SerializeField] GameManager gameManager;

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
	}
}
