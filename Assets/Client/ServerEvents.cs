using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerEvents : MonoBehaviour
{
	//DO NOT CHANGE THIS FILE TO ADD EVENTS
	//change CustomEvents.cs instead, it has examples and everything
	//ask me if you really want to do it

	[SerializeField] UDPServer server;
	[SerializeField] GameObject otherClientPrefab;
	List<OtherClient> otherClientList = new List<OtherClient>();

	[HideInInspector] public float lerpPercent = 0;
	float pastUpdateTime;
	float timeBetweenUpdates;
	[SerializeField] bool dynamicPlayerLerp;
	
	public void sendEvent(string eventName, string[] data)
	{
		server.sendMessage("e" + "~" + eventName + "~" + combineStringArray(data, "~"));
	}

	private void Update()
	{
		if (dynamicPlayerLerp)
		{
			lerpPercent = (Time.time - pastUpdateTime)/timeBetweenUpdates;
		}
		else
		{
			lerpPercent = (Time.time - pastUpdateTime) / (1/(float)server.transformTPS);
		}
	}
	public void rawEvents(string rawEvents)
	{
		string[] splitRawEvents = rawEvents.Split('|');
		for (int eventID = 0; eventID < splitRawEvents.Length - 1; eventID++)
		{
			if (splitRawEvents[eventID] != "")
			{
				string[] peices = splitRawEvents[eventID].Split("~");
				if(this != null) //events get recieved even after exit, showing a ton of annoying errors (this removes that)
				{
					try
					{
						this.SendMessage(peices[0], sliceStringArray(peices, 1, peices.Length));
					}
					catch (Exception e)
					{
						Debug.LogWarning(e.Message + ", no event for received event: " + splitRawEvents[eventID]);
					}
				}
			}
		}
	}

	public void restartLerpTimer()
	{
		timeBetweenUpdates = Time.time - pastUpdateTime;
		pastUpdateTime = Time.time;
	}

	//events --------------------------------------------------------------
	void u(string[] data)
	{
		int clientID = int.Parse(data[0]);
		Vector3 position = parseVector3(data[1]);
		Quaternion rotation = parseQuaternion(data[2]);

		if (clientID != UDPServer.ID)
		{
			foreach (OtherClient otherClient in otherClientList)
			{
				if(otherClient.ID == clientID)
				{
					otherClient.setTransform(position, rotation);
				}
			}
		}
	}

	void removeClient(string[] data)
	{
		int clientID = int.Parse(data[0]);

		if(clientID == UDPServer.ID)
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

		OtherClient newClientScript = Instantiate(otherClientPrefab).GetComponent<OtherClient>();
		otherClientList.Add(newClientScript);
		newClientScript.setInfo(newClientID, newClientUsername);
		sendEvent("serverMessage", new string[] { newClientUsername + " joined the game" });

		this.SendMessage("onPlayerConnect", newClientID);
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

	public int getIDByOtherClientScript(OtherClient otherClient1)
	{
		return otherClient1.ID;
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
