using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Threading;
using UnityEngine.UI;

public class ServerOption : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI pingText;
	[SerializeField] TextMeshProUGUI playersText;
	[SerializeField] TextMeshProUGUI versionText;
	[SerializeField] GameObject serverOffline;

	UdpClient udpClient;
	IPEndPoint remoteEndPoint;

	public bool online = false;
	Lobby lobby;

	public int port;
	public string ip;

	float startTime;

	private void Start()
	{
		lobby = GameObject.Find("LobbyHandler").GetComponent<Lobby>();
		initUDP();
	}

	public void selectServer()
	{
		lobby.setSelectedServer(this);
	}

	public void refreshInfo()
	{
		try
		{
			startTime = Time.time;
			sendUDPMessage("ping");
		}
		catch
		{
			serverOffline.SetActive(true);
			versionText.text = "";
			playersText.text = "";
			pingText.text = "";
			online = false;
		}
	}

	async void udpReciever()
	{
		while (true)
		{
			byte[] receiveBytes = new byte[0];
			await Task.Run(() => receiveBytes = udpClient.Receive(ref remoteEndPoint));

			Debug.Log("Start: " + startTime + " Current: " + Time.time);
			string recieveString = Encoding.ASCII.GetString(receiveBytes);
			Debug.Log("Recieved Message from " + ip + ": " + recieveString);
			float ping = (int)((Time.time - startTime) * 1000); //get ping
			serverOffline.SetActive(false);
			versionText.text = "?";//"V" + recieveString;
			playersText.text = "?";
			pingText.text = ping + "ms";
			online = true;
		}
	}

	void initUDP()
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

		udpClient = new UdpClient();
		udpClient.Connect(ip, port);

		udpReciever();
	}

	public void sendUDPMessage(string message)
	{
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

		//send message
		udpClient.Send(udpData, udpData.Length);
	}
}
