using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
	[SerializeField] ServerEvents serverEvents;
	[SerializeField] Chat chat;
	[SerializeField] AudioPlayer audioPlayer;

	[SerializeField] PlayerManager playerManager;

	[SerializeField] BulletManager bulletManager;
	[SerializeField] KillFeed killFeed;
	[SerializeField] AbilityManager abilityManager;
	[SerializeField] WeaponSwitcher weaponSwitcher;



	//example:
	public void sendExampleEventExample()
	{
		//data has to be in an array of strings (convert all data types to string and put in an array like shown)
		string[] data = { 1 + "", .54 + "", "bruh" };

		//there are 4 event sending functions:
		//sendGlobalEvent(eventType, data) - sends an event to everyone, even yourself
		//sendDirectEvent(eventType, data, targetClientID) - sends an event to the specified client id
		//sendEventToOtherClients(eventType, data) - sends an event to everyone else excluding yourself
		//sendServerEvent(eventType, data) - sends an event to the server (you won't be using this)

		//send event
		//since this is a global event, every other client will get that event
		serverEvents.sendGlobalEvent("exampleEvent", data);

		//then make a recieve function (example is the function below this one) that gets called when an event is recieved by this client
		//the data perameter will have the same variables as the data sent using the event send functions
	}

	//function name is the same as event type sent, only perameter is string array (both rules must be followed for things to work!!!)
	public void exampleEvent(string[] data)
	{
		//you can then parse the data recieved in the string array
		int exampleData1 = int.Parse(data[0]);
		float exampleData2 = float.Parse(data[1]);
		string exampleData3 = data[2];

		//do things with data
		Debug.Log(exampleData1 + ", " + exampleData2 + ", " + exampleData3);
	}

	public void changeCoins(string[] data)
	{
		int coinsGotten = int.Parse(data[0]);
		playerManager.addCoins(coinsGotten);
	}

	public void quickSettings(string[] data)
	{
		foreach(string settings in data)
		{
			if(!string.IsNullOrWhiteSpace(settings))
			{
				string[] settingsPeices = settings.Split(';');
				string settingsTarget = settingsPeices[0];
				if(settingsTarget == "ability")
				{
					abilityManager.newSettings(settingsPeices);
				}
				else if(settingsTarget == "player")
				{
					playerManager.newSettings(settingsPeices);
				}
				else if(settingsTarget == "weapon")
				{
					int targetID = int.Parse(settingsPeices[1]);
					WeaponSystem weaponBeingChanged = WeaponSwitcher.Instance.GetItemThroughID(targetID).GetComponent<WeaponSystem>();
					weaponBeingChanged.newSettings(settingsPeices);
				}
				else
				{
					Debug.LogError("Could not find settings target: " + settingsTarget);
				}
			}
		}
	}

	public void newKillFeed(string[] data)
	{
		string attacker = data[0];
		string attacked = data[1];
		int verbIndex = int.Parse(data[2]);
		int adverbIndex = int.Parse(data[3]);

		killFeed.newFeed(attacker, attacked, verbIndex, adverbIndex);
	}

	public void chatMessage(string[] data)
	{
		string username = data[0];
		string message = data[1];

		chat.newMessage(username, message);
	}

	public void playAudio(string[] data)
	{
		int clipID = int.Parse(data[0]);
		Vector3 position = ServerEvents.parseVector3(data[1]);
		float volume = float.Parse(data[2]);
		float pitch = float.Parse(data[3]);
		int parentClientID = int.Parse(data[4]);

		if(parentClientID == Client.ID) //this client is parent
		{
			audioPlayer.spawnAudio(audioPlayer.getClipByID(clipID), position, volume, pitch, playerManager.transform);
		}
		else if(parentClientID != -1) //another client is parent
		{
			audioPlayer.spawnAudio(audioPlayer.getClipByID(clipID), position, volume, pitch, serverEvents.getOtherClientScriptByID(parentClientID).transform);
		}
		else //no parent
		{
			audioPlayer.spawnAudio(audioPlayer.getClipByID(clipID), position, volume, pitch);
		}

	}

	public void onPlayerConnect(int clientID)
	{
		string clientUsername = serverEvents.getUsername(clientID);
		chat.serverMessage(clientUsername + " has joined the game");

		if(clientID < Client.ID)
		{
			Client.owner = false;
			Debug.Log(clientID + " is the new owner");
		}
	}

	public void onPlayerDisconnect(string clientUsername)
	{
		chat.serverMessage(clientUsername + " has left the game");
	}

	public void onPlayerDisconnectID(int clientID)
	{
		if (clientID < Client.ID)
		{
			Client.owner = true;
			foreach (OtherClient otherClient in serverEvents.otherClientList)
			{
				if (otherClient.ID < Client.ID)
				{
					Client.owner = false;
					return;
				}
			}
			Debug.Log("You are new owner");
		}
	}

	public void switchGun(string[] data)
	{
		//not implimented
		int clientID = int.Parse(data[0]);
		int weaponID = int.Parse(data[1]);

		OtherClient otherClientScript = serverEvents.getOtherClientScriptByID(clientID);
		otherClientScript.setEquippedWeapon(weaponID);
	}

	public void damage(string[] data)
	{
		int damage = int.Parse(data[0]);
		int attackedLife = int.Parse(data[1]);
		int attackerID = int.Parse(data[2]);

		playerManager.TakeDamage(damage, attackedLife, attackerID);
	}

	public void setHealth(string[] data)
	{
		int clientID = int.Parse(data[0]);
		int health = int.Parse(data[1]);
		int currentLife = int.Parse(data[2]);

		OtherClient otherClientScript = serverEvents.getOtherClientScriptByID(clientID);

		otherClientScript.SetHealth(health, currentLife);
	}

	public void setClientTeam(string[] data)
	{
		int clientID = int.Parse(data[0]);
		int team = int.Parse(data[1]);
		bool isResponse = bool.Parse(data[2]);

		OtherClient otherClientScript = serverEvents.getOtherClientScriptByID(clientID);
		otherClientScript.setTeam(team);

		if(!isResponse)
		{
			serverEvents.sendDirectEvent("setClientTeam", new string[] { Client.ID + "", PlayerManager.team + "", "true" }, clientID);
		}
	}

	public void spawnBulletEvent(string[] data)
	{
		Vector3 bulletPos = ServerEvents.parseVector3(data[0]);
		Vector3 bulletVel = ServerEvents.parseVector3(data[1]);

		bulletManager.spawnBullet(bulletPos, bulletVel);
	}

	public void resetAbilities(string[] data)
	{
		string resetter = data[0];

		abilityManager.resetUpgrades();
		chat.serverMessage(resetter + " reset multipliers");
	}

	public void newDroppedWeapon(string[] data)
	{
		int weaponID = int.Parse(data[0]);
		int dropID = int.Parse(data[1]);
		Vector3 position = ServerEvents.parseVector3(data[2]);
		Vector3 velocity = ServerEvents.parseVector3(data[3]);
		Vector3 forceAdded = ServerEvents.parseVector3(data[4]);
		Vector3 torqueAdded = ServerEvents.parseVector3(data[5]);
		int ammoInReserve = int.Parse(data[6]);
		int currentAmmo = int.Parse(data[7]);
		int maxAmmo = int.Parse(data[8]);

		weaponSwitcher.createDropItem(weaponID, dropID, position, velocity, forceAdded, torqueAdded, ammoInReserve, currentAmmo, maxAmmo);
	}

	public void pickedUpWeapon(string[] data)
	{
		int dropID = int.Parse(data[0]);
		weaponSwitcher.otherPlayerPickedUpWeapon(dropID);
	}
}
