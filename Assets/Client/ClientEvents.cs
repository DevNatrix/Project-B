using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClientEvents : MonoBehaviour
{
	public Client client;
	public int ID;

	private void Start()
	{
		InvokeRepeating("sendTestEvent", 1f, 1f);
		//sendTestEvent();
	}

	public void newEvent(string eventType, string[] eventInfo)
	{
		client.sendTCPMessage(eventType + "~" + gatherStringArray(eventInfo));
	}

	void sendTestEvent()
	{
		Debug.Log("Sent test event");
		string[] data = { "hi", "howdy", "breh" };
		newEvent("testEvent", data);
	}

	public void testEvent(string[] data)
	{
		Debug.Log("Test Event: " + gatherStringArray(data, " "));
	}

	public void clientInfo(string[] data)
	{
		Debug.Log(data[0]);
		ID = int.Parse(data[0]);
		Debug.Log("Client id: " + ID);
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


	//Utill -----------
	public static string gatherStringArray(string[] array, string devider = "~")
	{
		//combine all
		string finalString = "";
		foreach (string part in array)
		{
			finalString += part + devider;
		}

		//get rid of trailing ~
		finalString = finalString.Substring(0, finalString.Length - devider.Length);

		return finalString;
	}
	public static string[] sliceStringArray(string[] arrayItem, int start, int end)
	{
		string[] finalArray = new string[arrayItem.Length];
		for (int i = start; i < end; i++)
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
