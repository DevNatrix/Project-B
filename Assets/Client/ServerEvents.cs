using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerEvents : MonoBehaviour
{
	//DO NOT CHANGE THIS FILE TO ADD EVENTS
	//change CustomEvents.cs instead, it has examples and everything
	//ask me if you really want to do it

	[SerializeField] Client client;
	[SerializeField] GameObject otherClientPrefab;
	List<OtherClient> otherClientList = new List<OtherClient>();

	[HideInInspector] public float lerpPercent = 0;
	float pastUpdateTime;
	float timeBetweenUpdates;
	[SerializeField] bool dynamicPlayerLerp;

	[SerializeField] PlayerManager playerManager;

	public void sendGlobalEvent(string eventType, string[] eventInfo)
	{
		client.sendTCPMessage("g~" + eventType + "~" + combineStringArray(eventInfo, "~"));
	}
	public void sendEventToOtherClients(string eventType, string[] eventInfo)
	{
		client.sendTCPMessage("o~" + eventType + "~" + combineStringArray(eventInfo, "~"));
	}
	public void sendDirectEvent(string eventType, string[] eventInfo, int targetClient)
	{
		client.sendTCPMessage("d~" + targetClient + "~" + eventType + "~" + combineStringArray(eventInfo, "~"));
	}
	public void sendServerEvent(string eventType, string[] eventInfo)
	{
		client.sendTCPMessage("s~" + eventType + "~" + combineStringArray(eventInfo, "~"));
	}

	public void rawEvent(string message)
	{
		string[] peices = message.Split('~');
		try
		{
			//trigger event (it also goes to custom events if can't find it here)
			this.SendMessage(peices[0], sliceStringArray(peices, 1, peices.Length));
		}
		catch (Exception e)
		{
			Debug.LogWarning(e.Message + ", no event for received event: " + message);
		}
	}

	//events --------------------------------------------------------------

	void removeClient(string[] data)
	{
		int clientID = int.Parse(data[0]);
		string clientUsername = getUsername(clientID);

		foreach (OtherClient otherClient in otherClientList)
		{
			if (otherClient.ID == clientID)
			{
				otherClientList.Remove(otherClient);
				Destroy(otherClient.gameObject);
				this.SendMessage("onPlayerDisconnect", clientUsername);
				return;
			}
		}
	}

	void newClient(string[] data)
	{
		int clientID = int.Parse(data[0]);
		string newClientUsername = data[1];
		bool isResponseMessage = bool.Parse(data[2]);

		OtherClient newClientScript = Instantiate(otherClientPrefab).GetComponent<OtherClient>();
		otherClientList.Add(newClientScript);
		newClientScript.setInfo(clientID, newClientUsername);

		this.SendMessage("onPlayerConnect", clientID);

		if (!isResponseMessage)
		{
			sendDirectEvent("newClient", new string[] { Client.ID + "", Client.username, "true"}, clientID);
		}
	}

	public void setClientInfo(string[] data)
	{
		Client.ID = int.Parse(data[0]);
		playerManager.team = int.Parse(data[1]);
		
		Debug.Log("Client id: " + Client.ID);

		sendEventToOtherClients("newClient", new string[] { Client.ID + "", Client.username, "false"});
	}


	//tools --------------------------------------------------------------

	public OtherClient getOtherClientScriptByID(int clientID)
	{
		foreach (OtherClient otherClient in otherClientList)
		{
			if (otherClient.ID == clientID)
			{
				return otherClient;
			}
		}
		return null;
	}

	public int getIDByOtherClientScript(OtherClient otherClient)
	{
		return otherClient.ID;
	}

	public string getUsername(int clientID)
	{
		foreach (OtherClient otherClient in otherClientList)
		{
			if (otherClient.ID == clientID)
			{
				return otherClient.username;
			}
		}
		return null;
	}

	public static string combineStringArray(string[] arrayItem, string seperator = "")
	{
		string finalString = "";
		foreach(string item in arrayItem)
		{
			finalString += item + seperator;
		}
		return finalString;
	}

	public static string[] sliceStringArray(string[] arrayItem, int start, int end)
	{
		string[] finalArray = new string[arrayItem.Length];
		for(int i = start; i < end; i++)
		{
			finalArray[i - start] = arrayItem[i];
		}
		return finalArray;
	}
	public static Vector3 parseVector3(string vector3String)
	{
		vector3String = vector3String.Substring(1, vector3String.Length - 2); //get rid of parenthisis
		string[] parts = vector3String.Split(',');
		return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
	}
	public static Quaternion parseQuaternion(string quaternionString)
	{
		quaternionString = quaternionString.Substring(1, quaternionString.Length - 2); //get rid of parenthisis
		string[] parts = quaternionString.Split(',');
		return new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
	}
}
