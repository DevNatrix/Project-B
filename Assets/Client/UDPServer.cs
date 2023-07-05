using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

public class UDPServer : MonoBehaviour
{
	UdpClient client;
	IPEndPoint remoteEndPoint;
	int SERVERPORT;
	string SERVERADDRESS;
	[HideInInspector] public int ID;
	int TPS;
	public int latency = 0;
	[SerializeField] int messageTimoutMS = 1000;
	[SerializeField] Transform playerTransform;
	[SerializeField] ServerEvents serverEvents;

	public async void connectToServer(string username, int serverPort, string serverAddress, int clientPort = -1)
	{
		SERVERADDRESS = serverAddress;
		SERVERPORT = serverPort;

		//attempt connect
		try
		{
			Debug.Log("Setting server...");
			if (clientPort == -1)
			{
				client = new UdpClient();
			}
			else
			{
				client = new UdpClient(clientPort);
			}
			client.Connect(SERVERADDRESS, SERVERPORT);
			remoteEndPoint = new IPEndPoint(IPAddress.Any, SERVERPORT);

			Debug.Log("Server set, Sending Join Message...");
			sendMessage("newClient~" + username);

			//wait for response
			byte[] receiveBytes = new byte[0];
			await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(1000));
			Debug.Log("Accepted, processing data...");
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
			Debug.Log("Data: " + recieveString);
			ID = int.Parse(recieveString.Split('~')[0]);
			TPS = int.Parse(recieveString.Split('~')[1]);

			Debug.Log("User ID: " + ID);
			Debug.Log("Given TPS: " + TPS);

			//start main update loop
			serverEvents.timeBetweenUpdates = 1 / (float)TPS;
			InvokeRepeating("serverUpdater", 0, 1 / (float)TPS);
		}
		catch (Exception e)
		{
			Debug.LogError("Couldn't connect to server: " + e.Message);
			return;
		}
	}


	async void serverUpdater()
	{
		//send
		sendMessage("u~" + ID + "~" + playerTransform.position + "~" + playerTransform.eulerAngles);

		//recieve
		string info = "";
		byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");

		float latencyTimer = Time.time;
		await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(messageTimoutMS));
		latency = (int)Mathf.Round((Time.time - latencyTimer) * 1000);
		info = Encoding.ASCII.GetString(receiveBytes);

		//processing response
		serverEvents.rawEvents(info);
	}

	public void sendMessage(string message)
	{
		//Debug.Log("Sent: " + message);
		byte[] sendBytes = Encoding.ASCII.GetBytes(message);
		client.Send(sendBytes, sendBytes.Length);
	}

}
