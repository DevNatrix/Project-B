using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading;

public class UDPServer : MonoBehaviour
{
	[HideInInspector] public UdpClient clientE;
	[HideInInspector] public UdpClient clientT;
	IPEndPoint remoteEndPoint;
	int SERVERPORT;
	string SERVERADDRESS;
	[HideInInspector] public static int ID;
	[HideInInspector] public int transformTPS;
	[HideInInspector] public int eventTPS;
	[SerializeField] int maxDroppedMessagesForDisconnect;
	[SerializeField] int messageTimoutMS = 1000;
	[SerializeField] Transform playerTransform;
	[SerializeField] Transform playerCamTransform;
	[SerializeField] ServerEvents serverEvents;


	[Header("Debug:")]
	[SerializeField] TextMeshProUGUI outgoingMessagesText;
	[SerializeField] TextMeshProUGUI droppedMessagesText;
	[SerializeField] TextMeshProUGUI sendBytesText;
	[SerializeField] TextMeshProUGUI recieveBytesText;
	[SerializeField] TextMeshProUGUI packetsText;
	[SerializeField] TextMeshProUGUI latencyText;
	[SerializeField] TextMeshProUGUI currentFPSText;
	[SerializeField] TextMeshProUGUI minFPSText;
	[SerializeField] TextMeshProUGUI maxFPSText;
	[SerializeField] TextMeshProUGUI messageIDText;

	int outgoingMessages = 0;
	int droppedMessages = 0;
	int sendBytesCount = 0;
	int recieveBytesCount = 0;
	public static int latency = 0;
	int packets = 0;
	int currentUMessageID = 0;
	int maxMessageID;

	int FPS = 0;
	int minFPS = 0;
	int maxFPS = 0;

	public static bool lostConnection = false;


	private void Update()
	{
		FPS++;

		int currentFPS = (int) (1 / Time.deltaTime);
		if (currentFPS > maxFPS)
		{
			maxFPS = currentFPS;
		}

		if (currentFPS < minFPS)
		{
			minFPS = currentFPS;
		}
	}

	public void updateDebugInterval()
	{
		droppedMessagesText.text = "Droped: " + droppedMessages;
		sendBytesText.text = "Sent Bytes: " + sendBytesCount;
		recieveBytesText.text = "Recieve Bytes: " + recieveBytesCount;
		packetsText.text = packets + " / " + (transformTPS + eventTPS) + " TPS";

		currentFPSText.text = "FPS: " + FPS;
		minFPSText.text = "Min FPS: " + minFPS;
		maxFPSText.text = "Max FPS: " + maxFPS;

		droppedMessages = 0;
		sendBytesCount = 0;
		recieveBytesCount = 0;
		packets = 0;

		minFPS = FPS;
		maxFPS = FPS;
		FPS = 0;

		if (droppedMessages > maxDroppedMessagesForDisconnect)
		{
			lostConnection = true;
			SceneManager.LoadScene("Lobby");
		}
	}

	public void updateDebug()
	{
		latencyText.text = "Latency (ms): " + latency;
		outgoingMessagesText.text = "Outgoing: " + outgoingMessages;
	}

	private void Start()
	{
		if (Lobby.bestPort != -1)
		{
			connectToServer(Lobby.username, Lobby.bestPort, Lobby.bestIP);
		}
		else
		{
			SceneManager.LoadScene("Lobby");
		}
	}

	public async void connectToServer(string username, int serverPort, string serverAddress, int clientPort = -1)
	{
		SERVERADDRESS = serverAddress;
		SERVERPORT = serverPort;

		//attempt connect
		try
		{
			//client t for transforms, client e for events
			Debug.Log("Setting server...");
			clientT = new UdpClient();
			clientE = new UdpClient();
			clientT.Connect(SERVERADDRESS, SERVERPORT);
			clientE.Connect(SERVERADDRESS, SERVERPORT);
			remoteEndPoint = new IPEndPoint(IPAddress.Any, SERVERPORT);

			Debug.Log("Server set, Sending Join Message...");
			sendMessage("newClient~" + username, clientE);

			//wait for response
			byte[] receiveBytes = new byte[0];
			await Task.WhenAny(Task.Run(() => receiveBytes = clientE.Receive(ref remoteEndPoint)), Task.Delay(1000));
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
			print(recieveString);
			ID = int.Parse(recieveString.Split('~')[0]);
			transformTPS = int.Parse(recieveString.Split('~')[1]);
			eventTPS = int.Parse(recieveString.Split('~')[2]);
			currentUMessageID = 0;
			maxMessageID = int.Parse(recieveString.Split('~')[3]);

			Debug.Log("User ID: " + ID);
			Debug.Log("Given Transform TPS: " + transformTPS);
			Debug.Log("Given Event TPS: " + eventTPS);

			//start main update loop
			InvokeRepeating("transformUpdater", 0, 1 / (float)transformTPS);
			InvokeRepeating("eventUpdater", 0, 1 / (float)eventTPS);
			InvokeRepeating("updateDebugInterval", 0, 1);
		}
		catch (Exception e)
		{
			Debug.LogError("Couldn't connect to server: " + e.Message);
			return;
		}
	}


	async void transformUpdater()
	{
		//send
		float startTime = Time.time;
		sendMessage("tu~" + ID + "~" + playerTransform.position + "~" + playerCamTransform.rotation, clientT);
		outgoingMessages++;
		packets++;

		//recieve
		string info = "";
		byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");
		Task receiveTask = Task.Run(() => receiveBytes = clientT.Receive(ref remoteEndPoint));
		Task timeoutTask = Task.Delay(messageTimoutMS);
		await Task.WhenAny(receiveTask, timeoutTask);
		info = Encoding.ASCII.GetString(receiveBytes);
		outgoingMessages--;

		if(!timeoutTask.IsCompleted && info != "EMPTY")
		{
			//update message id
			string[] splitRawEvents = info.Split('|');
			currentUMessageID++;
			int gottenUMessageID;
			try
			{
				gottenUMessageID = int.Parse(splitRawEvents[splitRawEvents.Length - 1]);
			}
			catch
			{
				Debug.LogError("Bad message: " + info);
				return;
			}

			//message id looping
			if(currentUMessageID >= maxMessageID)
			{
				currentUMessageID = 0;
			}

			//making sure it's the right id (in the right order)
			if (gottenUMessageID != currentUMessageID)
			{
				Debug.LogWarning("Update message recieved out of order ------ recieved: " + gottenUMessageID + ", current: " + currentUMessageID);
				if(gottenUMessageID > currentUMessageID)
				{
					//if not in the right order (and is ahead)
					currentUMessageID = gottenUMessageID;
				}
				else
				{
					//if not in the right order (and is behind)
					return;
				}
			}


			//process info
			latency = (int)Mathf.Round((Time.time - startTime) * 1000);
			recieveBytesCount += receiveBytes.Length;
			messageIDText.text = "U-Message ID: " + currentUMessageID;
			serverEvents.restartLerpTimer();
			serverEvents.rawEvents(info);
		}
		else
		{
			droppedMessages++;
		}
		updateDebug();
	}

	async void eventUpdater()
	{
		//float startTime = Time.time;
		//send
		sendMessage("eu~" + ID, clientE);
		outgoingMessages++;
		packets++;

		//recieve
		string info = "";
		byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");
		await Task.WhenAny(Task.Run(() => receiveBytes = clientE.Receive(ref remoteEndPoint)), Task.Delay(messageTimoutMS));
		//latency = (int)Mathf.Round((Time.time - startTime) * 1000); //get ping
		outgoingMessages--;
		info = Encoding.ASCII.GetString(receiveBytes);

		//processing response
		if(info != "EMPTY")
		{
			recieveBytesCount += receiveBytes.Length;
			serverEvents.rawEvents(info);
		}
		else
		{
			droppedMessages++;
		}
		updateDebug();
	}

	public void sendMessage(string message, UdpClient client)
	{
		//Debug.Log("Sent: " + message);
		byte[] sendBytes = Encoding.ASCII.GetBytes(message);
		sendBytesCount += sendBytes.Length;
		client.Send(sendBytes, sendBytes.Length);
	}

}
