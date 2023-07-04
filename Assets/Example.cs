using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
	[SerializeField] UDPServer server;
	[SerializeField] int serverPort;
	[SerializeField] string serverIP;
	void Start()
	{
		server.connectToServer("Username", serverPort, serverIP);
	}
}
