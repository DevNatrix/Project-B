using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
using System;

public class Lobby : MonoBehaviour
{
	UdpClient client;
	IPEndPoint remoteEndPoint;

	public static int bestPort;
	public static string bestIP;
	float bestPing = -1;

	[SerializeField] List<ServerOption> possibleServers;
	[SerializeField] int serverTimoutMS;
	[SerializeField] int currentServerVersion;
	[SerializeField] bool rejectWrongVersionServer;

	public void AutoJoin()
    {
		foreach(ServerOption option in possibleServers)
		{
			testConnection(option.port, option.ip);
		}
		Invoke("ChangeScene", ((float)serverTimoutMS)/1000);
	}

	public void ChangeScene()
	{
		if (bestPing != -1)
		{
			Debug.Log("Best Server ----> IP: " + bestIP + ", Port: " + bestPort + ", Ping: " + bestPing);
			SceneManager.LoadScene("Main");
		}
		else
		{
			Debug.LogError("No Servers Found");
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
			await Task.WhenAny(Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint)));
			float ping = Time.time - startTime; //get ping
			string recieveString = Encoding.ASCII.GetString(receiveBytes);

			//process
			bool rightVersion = int.Parse(recieveString) == currentServerVersion;
			if(rejectWrongVersionServer && rightVersion || !rejectWrongVersionServer)
			{
				if(ping < bestPing || bestPing == -1)
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
    }
}
