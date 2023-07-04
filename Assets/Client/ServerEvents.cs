using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEvents : MonoBehaviour
{
	[SerializeField] UDPServer server;
	[SerializeField] GameObject otherClientPrefab;
	List<OtherClient> otherClientList = new List<OtherClient>();
    /*public void sendEvent(string eventName, string data)
	{
		server.sendMessage(eventName + "~" + data);
	}*/ //for the future where I add event sending

	public void processEvent(string message)
	{
		print("Processed event: " + message);
		string[] splitEvent = message.Split("~");
		string eventType = splitEvent[0];
		switch (eventType)
		{
			case "u":
				updateTransform(int.Parse(splitEvent[1]), parseVector3(splitEvent[2]), parseVector3(splitEvent[3])); //client id, position, rotation
				break;
			case "newClient":
				newClient(int.Parse(splitEvent[1]), splitEvent[2]); //client id, username
				break;
		}
	}

	void updateTransform(int clientID, Vector3 position, Vector3 rotation)
	{
		if(clientID != server.ID)
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

	void newClient(int newClientID, string newClientUsername)
	{
		OtherClient newClientScript = Instantiate(otherClientPrefab).GetComponent<OtherClient>();
		otherClientList.Add(newClientScript);
		newClientScript.setInfo(newClientID, newClientUsername);
	}

	public void rawEvents(string rawEvents)
	{
		string[] splitRawEvents = rawEvents.Split('|');
		for (int eventID = 0; eventID < splitRawEvents.Length; eventID++)
		{
			if (splitRawEvents[eventID] != "")
			{
				try
				{
					processEvent(splitRawEvents[eventID]);
				}
				catch (Exception error)
				{
					Debug.LogError(error);
				}
			}
		}
	}

	//tools 
	public Vector3 parseVector3(string vector3String)
	{
		vector3String = vector3String.Substring(1, vector3String.Length - 2); //get rid of parenthisis
		string[] parts = vector3String.Split(',');
		return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
	}
	public Quaternion parseQuaternion(string quaternionString)
	{
		quaternionString = quaternionString.Substring(1, quaternionString.Length - 2); //get rid of parenthisis
		string[] parts = quaternionString.Split(',');
		return new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
	}
}
