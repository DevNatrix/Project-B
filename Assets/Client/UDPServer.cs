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

public class UDPServer : MonoBehaviour
{
	UdpClient client;
	IPEndPoint remoteEndPoint;
	int SERVERPORT;
	string SERVERADDRESS;
	[HideInInspector] public int ID;
	[HideInInspector] public int transformTPS;
	[HideInInspector] public int eventTPS;
	[SerializeField] int maxOutgoingMessages;
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
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
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
		float startTime = Time.time;
		//send
		sendMessage("tu~" + ID + "~" + playerTransform.position + "~" + playerCamTransform.rotation);
		outgoingMessages++;
		packets++;

		//recieve
		string info = "";
		byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");
		await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(messageTimoutMS));
		latency = (int)Mathf.Round((Time.time - startTime) * 1000); //get ping
		info = Encoding.ASCII.GetString(receiveBytes);
		outgoingMessages--;

		string[] splitRawEvents = info.Split('|');
		int gottenUMessageID;
		try
		{
			currentUMessageID++;
			gottenUMessageID = int.Parse(splitRawEvents[splitRawEvents.Length - 1]);
			if(currentUMessageID >= maxMessageID)
			{
				currentUMessageID = 0;
			}
			messageIDText.text = "U-Message ID: " + currentUMessageID;
			if (gottenUMessageID != currentUMessageID)
			{
				Debug.LogWarning("Update message recieved out of order ------ recieved: " + gottenUMessageID + ", current: " + currentUMessageID);
				if(gottenUMessageID > currentUMessageID)
				{
					currentUMessageID = gottenUMessageID;
				}
				else
				{
					return;
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + ", message: " + info);
		}


		//processing response
		if (info != "EMPTY")
		{
			recieveBytesCount += receiveBytes.Length;
			serverEvents.rawEvents(info);
		}
		else
		{
			droppedMessages++;
		}
		updateDebug();
		serverEvents.restartLerpTimer();
	}

	async void eventUpdater()
	{
		float startTime = Time.time;
		//send
		sendMessage("eu~" + ID);
		outgoingMessages++;
		packets++;

		//recieve
		string info = "";
		byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");
		await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(messageTimoutMS));
		latency = (int)Mathf.Round((Time.time - startTime) * 1000); //get ping
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

	public void sendMessage(string message)
	{
		//Debug.Log("Sent: " + message);
		byte[] sendBytes = Encoding.ASCII.GetBytes(message);
		sendBytesCount += sendBytes.Length;
		client.Send(sendBytes, sendBytes.Length);
	}

}
