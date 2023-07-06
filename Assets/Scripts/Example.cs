using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Example : MonoBehaviour
{
	[SerializeField] UDPServer server;
	void Start()
	{
		if(Lobby.bestPort != -1)
		{
			server.connectToServer("Username", Lobby.bestPort, Lobby.bestIP);
		}
		else
		{
			SceneManager.LoadScene("Lobby");
		}
	}
}
