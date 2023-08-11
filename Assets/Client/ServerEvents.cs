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
	
	public void sendEvent(string eventName, string[] data)
	{
		client.sendTCPMessage(eventName + "~" + combineStringArray(data, "~"));
		//server.sendMessage("e" + "~" + eventName + "~" + combineStringArray(data, "~"), server.clientE);
	}

	private void Update()
	{
		if (dynamicPlayerLerp)
		{
			lerpPercent = (Time.time - pastUpdateTime)/timeBetweenUpdates;
		}
		else
		{
			lerpPercent = (Time.time - pastUpdateTime) / (1/(float)client.transformTPS);
		}
	}

	public void rawEvent(string message)
	{
		string[] peices = message.Split('~');
		try
		{
			this.SendMessage(peices[0], sliceStringArray(peices, 1, peices.Length));
		}
		catch (Exception e)
		{
			Debug.LogWarning(e.Message + ", no event for received event: " + message);
		}
	}

	public void restartLerpTimer()
	{
		timeBetweenUpdates = Time.time - pastUpdateTime;
		pastUpdateTime = Time.time;
	}

	//events --------------------------------------------------------------

	void removeClient(string[] data)
	{
		int clientID = int.Parse(data[0]);

		if(clientID == Client.ID)
		{
			Debug.LogError("Server sent leave event for this client, closing game");
			Application.Quit();
		}
		foreach (OtherClient otherClient in otherClientList)
		{
			if (otherClient.ID == clientID)
			{
				sendEvent("serverMessage", new string[] { otherClient.username + " left the game" });
				otherClientList.Remove(otherClient);
				Destroy(otherClient.gameObject);
				this.SendMessage("onPlayerDisconnect", clientID);
				return;
			}
		}
	}

	void newClient(string[] data)
	{
		int newClientID = int.Parse(data[0]);
		string newClientUsername = data[1];
		bool isResponseJoin = bool.Parse(data[2]);

		if(newClientID == Client.ID)
		{
			return;
		}


		OtherClient newClientScript = Instantiate(otherClientPrefab).GetComponent<OtherClient>();
		otherClientList.Add(newClientScript);
		newClientScript.setInfo(newClientID, newClientUsername);
		sendEvent("serverMessage", new string[] { newClientUsername + " joined the game" });

		this.SendMessage("onPlayerConnect", newClientID);

		if (!isResponseJoin)
		{
			sendEvent("newClient", new string[] { Client.ID + "", Client.username, "TRUE" });
		}
	}

	public void clientInfo(string[] data)
	{
		Debug.Log(data[0]);
		Client.ID = int.Parse(data[0]);
		Debug.Log("Client id: " + Client.ID);

		sendEvent("newClient", new string[] { Client.ID + "", Client.username, "FALSE" });
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
