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
	[SerializeField] GameObject serverButton;

	UdpClient udpClient;
	IPEndPoint remoteEndPoint;

	Lobby lobby;

	public bool online = false;
	public int udpPort = 6969;
	public int tcpPort = 4242;
	public string ip;
	public int latency;

	float startTime;

	private void Start()
	{
		lobby = GameObject.Find("LobbyHandler").GetComponent<Lobby>();
		initUDP();
		udpReciever();
	}

	public void selectServer()
	{
		lobby.setSelectedServer(serverButton, this);
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
			try
			{
				byte[] receiveBytes = new byte[0];
				await Task.Run(() => receiveBytes = udpClient.Receive(ref remoteEndPoint));
				string recieveString = Encoding.ASCII.GetString(receiveBytes);

				latency = (int)((Time.time - startTime) * 1000); //get ping
				online = true;
				serverOffline.SetActive(false);

				versionText.text = "?";//"V" + recieveString;
				playersText.text = "?";
				pingText.text = latency + "ms";
			}
			catch
			{
				serverOffline.SetActive(true);
				versionText.text = "";
				playersText.text = "";
				pingText.text = "";
				online = false;

				initUDP();
			}
		}
	}

	void initUDP()
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Any, udpPort);

		udpClient = new UdpClient();
		udpClient.Connect(ip, udpPort);
	}

	public void sendUDPMessage(string message)
	{
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

		//send message
		udpClient.Send(udpData, udpData.Length);
	}
}
