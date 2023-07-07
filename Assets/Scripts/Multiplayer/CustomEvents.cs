using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
	[SerializeField] ServerEvents serverEvents;
	[SerializeField] Chat chat;

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
}
