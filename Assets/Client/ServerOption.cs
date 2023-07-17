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

public class ServerOption : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI pingText;
	[SerializeField] TextMeshProUGUI playersText;
	[SerializeField] TextMeshProUGUI versionText;
	[SerializeField] GameObject serverOffline;

	UdpClient client;
	IPEndPoint remoteEndPoint;

	public async void refreshInfo(int timeoutMS)
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

			//wait for response or if a timout happens
			byte[] receiveBytes = Encoding.ASCII.GetBytes("EMPTY");
			Task receiveTask = Task.Run(() => receiveBytes = client.Receive(ref remoteEndPoint));
			Task timeoutTask = Task.Delay(timeoutMS);
			await Task.WhenAny(receiveTask, timeoutTask);

			if(!receiveTask.IsCompleted)
			{
				serverOffline.SetActive(true);
				versionText.text = "";
				playersText.text = "";
				pingText.text = "";
			}
			else
			{
				string recieveString = Encoding.ASCII.GetString(receiveBytes);
				Debug.Log("Recieved Message from " + ip + ": " + recieveString);
				float ping = (int)((Time.time - startTime)*1000); //get ping
				serverOffline.SetActive(false);
				versionText.text = "V" + recieveString;
				playersText.text = "?";
				pingText.text = ping + "ms";
			}
		}
		catch (Exception e)
		{
			Debug.Log("Couldnt join server: " + e);
		}
	}

	public int port;
	public string ip;
}
