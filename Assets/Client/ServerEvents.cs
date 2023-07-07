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
			lerpPercent = (Time.time - pastUpdateTime) / (1/(float)server.TPS);
		}
	}
	public void rawEvents(string rawEvents)
	{
		timeBetweenUpdates = Time.time - pastUpdateTime;
		pastUpdateTime = Time.time;

		string[] splitRawEvents = rawEvents.Split('|');
		for (int eventID = 0; eventID < splitRawEvents.Length; eventID++)
		{
			if (splitRawEvents[eventID] != "")
			{
				string[] peices = splitRawEvents[eventID].Split("~");
				this.SendMessage(peices[0], sliceStringArray(peices, 1, peices.Length));
			}
		}
	}

	//events --------------------------------------------------------------

	void u(string[] data)
	{
		int clientID = int.Parse(data[0]);
		Vector3 position = parseVector3(data[1]);
		Quaternion rotation = parseQuaternion(data[2]);

		if (clientID != server.ID)
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

		if(clientID == server.ID)
		{
			Debug.LogError("Server sent leave event for this client, closing game");
			Application.Quit();
		}
		foreach (OtherClient otherClient in otherClientList)
		{
			if (otherClient.ID == clientID)
			{
				otherClientList.Remove(otherClient);
				Destroy(otherClient.gameObject);
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
	}


	//tools --------------------------------------------------------------

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
