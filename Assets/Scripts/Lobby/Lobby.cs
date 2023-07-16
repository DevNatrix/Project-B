using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using TMPro;
using UnityEditor;
using Steamworks;

public class Lobby : MonoBehaviour
{
	UdpClient client;
	IPEndPoint remoteEndPoint;

	public static int bestPort = -1;
	public static string bestIP;
	float bestPing;

	public static string username;

	[SerializeField] List<ServerOption> possibleServers;
	[SerializeField] int serverTimoutMS;
	[SerializeField] int currentServerVersion;
	[SerializeField] bool rejectWrongVersionServer;

	[SerializeField] TextMeshProUGUI statusText;
	[SerializeField] Button connectButton;
	[SerializeField] GameObject lostConnectionGUI;


	private void Start()
    {
		if(UDPServer.lostConnection)
		{
			UDPServer.lostConnection = false;
			lostConnectionGUI.SetActive(true);
		}
	}

	public void Deactivate(GameObject objectToDeactivate)
	{
		objectToDeactivate.SetActive(false);
	}

	private void Update()
    {
		username = SteamHandler.usernameSteam;
	}

    public void AutoJoin()
    {
			connectButton.enabled = false;
			statusText.text = "Pinging Servers...";
			foreach (ServerOption option in possibleServers)
			{
				testConnection(option.port, option.ip);
			}
			Invoke("ChangeScene", ((float)serverTimoutMS + 500) / 1000);
	}

	async void ChangeScene()
	{
		if (bestPort != -1)
		{
			Debug.Log("Best Server ----> IP: " + bestIP + ", Port: " + bestPort + ", Ping: " + bestPing);
			statusText.text = "Joining Server...";
			await Task.Delay(1000);
			SceneManager.LoadScene("Main");
		}
		else
		{
			statusText.text = "No Online Servers";
			Debug.LogError("No Online Servers");
			await Task.Delay(2000);
			connectButton.enabled = true;
			statusText.text = "Try Again";
		}
	}

	async void testConnection(int port, string ip)
	{
		try
		{
			//create udp connection
			client = new UdpClient();
			client.Connect(ip, port);
			remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

			//send message
			byte[] sendBytes = Encoding.ASCII.GetBytes("ping");
			float startTime = Time.time; //start ping timer
			client.Send(sendBytes, sendBytes.Length);

			//wait for response
			byte[] receiveBytes = new byte[0];
			await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)), Task.Delay(serverTimoutMS));
			float ping = Time.time - startTime; //get ping
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
			Debug.Log("Recieved Message from " + ip + ": " + recieveString);

			//process
			bool rightVersion = int.Parse(recieveString) == currentServerVersion;
			if(rejectWrongVersionServer && rightVersion || !rejectWrongVersionServer)
			{
				if(ping < bestPing || bestPort == -1)
				{
					bestPort = port;
					bestIP = ip;
					bestPing = ping;
				}
			}
			else
			{
				Debug.LogWarning("Rejected Server: ----> IP: " + ip + ", Port: " + port + ", Ping: " + ping + ", Reason: Wrong Version (current: V" + currentServerVersion + ", Server: V" + recieveString + ")");
			}

			Debug.Log("Possible Server: ----> IP: " + ip + ", Port: " + port + ", Ping: " + ping);
		}
		catch (Exception e)
		{
			Debug.Log("Rejected Server: ----> IP: " + ip + ", Port: " + port + ", Reason: " + e);
		}
	}

    public void ExitGame()
    {
        Application.Quit();

		/*if(EditorApplication.isPlaying == true)
        {
			EditorApplication.isPlaying = false;
		}*/ //doesnt let you build
	}
}
