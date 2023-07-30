using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
	[SerializeField] ServerEvents serverEvents;
	[SerializeField] Chat chat;
	[SerializeField] AudioPlayer audioPlayer;
	[SerializeField] LocalHealth localHealth;

	//function name is the same as event type sent, only perameter is string array
	public void exampleEvent(string[] data)
	{
		//set data
		int exampleData1 = int.Parse(data[0]);
		float exampleData2 = float.Parse(data[1]);
		string exampleData3 = data[2];

		//do things with data
		Debug.Log(exampleData1 + ", " + exampleData2 + ", " + exampleData3);
	}

	//doesnt have to have it's own function, just do the things inside of the function
	public void sendExampleEventExample()
	{
		//data has to be in an array of strings (convert all data types to string and put in an array like shown)
		string[] data = { 1 + "", .54 + "", "bruh" };
		serverEvents.sendEvent("exampleEvent", data);
	}

	public void chatMessage(string[] data)
	{
		string username = data[0];
		string message = data[1];

		chat.newMessage(username, message);
	}

	public void serverMessage(string[] data)
	{
		string message = data[0];
		chat.serverMessage(message);
	}

	public void playAudio(string[] data)
	{
		int clipID = int.Parse(data[0]);
		Vector3 position = ServerEvents.parseVector3(data[1]);
		float volume = float.Parse(data[2]);
		float pitch = float.Parse(data[3]);

		audioPlayer.createAudio(audioPlayer.getClipByID(clipID), position, volume, pitch);
	}

	public void onPlayerConnect(int clientID)
	{
		//get client username using serverEvents.getUsername
		print("join");
	}

	public void onPlayerDisconnect(int clientID)
	{
		print("leave");
	}

	public void switchGun(string[] data)
	{
		int clientID = int.Parse(data[0]);
		string weaponName = data[1];

		Debug.Log(clientID + ", " + weaponName);
	}

	public void Damage(string[] data)
	{
		int damage = int.Parse(data[0]);
		int clientID = int.Parse(data[1]);
		Debug.Log("Damage event");

		if (clientID == UDPServer.ID)
		{
			if (damage >= localHealth.health)
			{
				string[] sendData = { UDPServer.ID + "" };
				serverEvents.sendEvent("Death", sendData);
			}
			else
			{
				string[] sendData = { UDPServer.ID + "", (localHealth.health - damage) + "" };
				serverEvents.sendEvent("SetHealth", sendData);
				localHealth.TakeDamage(damage);
			}
		}
		else
        {
			OtherClient otherClientScript = serverEvents.getOtherClientScriptByID(clientID);
			Health otherClientHealth = otherClientScript.gameObject.GetComponent<Health>();
			
			otherClientHealth.TakeDamage(damage);
		}
	}

	public void SetHealth(string[] data)
	{
		int clientID = int.Parse(data[0]);
		int health = int.Parse(data[1]);

		if (clientID == UDPServer.ID)
		{
			//LocalHealth.Instance.health = health;
		}
		else
		{
			OtherClient otherClientScript = serverEvents.getOtherClientScriptByID(clientID);
			Health otherClientHealth = otherClientScript.gameObject.GetComponent<Health>();

			otherClientHealth.SetHealth(health);
		}
	}
}
