using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
	public int TCP_PORT = 4242;
	public int UDP_PORT = 6969;
	public string SERVER_IP = "127.0.0.1";

	public static bool lostConnection = false;

	IPEndPoint remoteEndPoint;
	UdpClient udpClient;

	TcpClient tcpClient;
	NetworkStream tcpStream;

	float udpPingStartTime;
	float tcpPingStartTime;
	float lastGottenPingTime;

	int udpLatency;
	int tcpLatency;
	int sendBytesUDP;
	int sendBytesTCP;
	int getBytesUDP;
	int getBytesTCP;

	TextMeshProUGUI udpPing;
	TextMeshProUGUI tcpPing;
	TextMeshProUGUI tcpSendBytes;
	TextMeshProUGUI udpSendBytes;
	TextMeshProUGUI tcpGetBytes;
	TextMeshProUGUI udpGetBytes;
	TextMeshProUGUI udpProcessErrorText;
	TextMeshProUGUI tcpProcessErrorText;

	public ServerEvents events;
	public AdvancedDebug advancedDebug;

	public Transform playerTransform;
	public Transform camTransform;

	public static int ID;
	public static int latency;
	public static string username;
	public static int transformTPS = 32;
	bool serverOnline = false;
	public bool showClient = true;
	int udpProcessErrors = 0;
	int tcpProcessErrors = 0;
	public float maxSecondsBeforeDisconnect = 3f;

	public static bool owner;

	private void Start()
	{

		lastGottenPingTime = Time.time;
		lostConnection = false;

		if (Lobby.bestUDPPort == -1)
		{
			SceneManager.LoadScene(0);
			return;
		}
		owner = true;
		Debug.Log("You are the server owner");

		UDP_PORT = Lobby.bestUDPPort;
		TCP_PORT = Lobby.bestTCPPort;
		username = Lobby.username;
		SERVER_IP = Lobby.bestIP;

		udpPing = advancedDebug.createDebug("UDP Ping");
		tcpPing = advancedDebug.createDebug("TCP Ping");
		udpSendBytes = advancedDebug.createDebug("UDP Send Bytes");
		tcpSendBytes = advancedDebug.createDebug("TCP Send Bytes");
		udpGetBytes = advancedDebug.createDebug("UDP Get Bytes");
		tcpGetBytes = advancedDebug.createDebug("TCP Get Bytes");
		udpProcessErrorText = advancedDebug.createDebug("UDP Process Errors");
		tcpProcessErrorText = advancedDebug.createDebug("TCP Process Errors");

		initUDP();
		initTCP();

		InvokeRepeating("Ping", 0, 1f);
		InvokeRepeating("DebugText", 1, 1f);
		InvokeRepeating("TransformUpdate", 0, 1/(float)transformTPS);
	}

	void Ping()
	{
		udpPingStartTime = Time.time;
		sendUDPMessage("ping");
		tcpPingStartTime = Time.time;
		sendTCPMessage("s~ping");
	}

	void DebugText()
	{
		tcpSendBytes.text = "TCP send b/s: " + sendBytesTCP;
		udpSendBytes.text = "UDP send b/s: " + sendBytesUDP;
		tcpGetBytes.text = "TCP get b/s: " + getBytesTCP;
		udpGetBytes.text = "UDP get b/s: " + getBytesUDP;
		udpProcessErrorText.text = "UDP Proc Errs: " + udpProcessErrors;
		tcpProcessErrorText.text = "TCP Proc Errs: " + tcpProcessErrors;

		sendBytesTCP = 0;
		sendBytesUDP = 0;
		getBytesTCP = 0;
		getBytesUDP = 0;
		udpProcessErrors = 0;
		tcpProcessErrors = 0;
	}

	void TransformUpdate()
	{
		sendUDPMessage(Client.ID + "~" + playerTransform.position + "~" + camTransform.rotation + "~" + showClient + "~" + Movement.crouching);
	}

	void initUDP()
	{
		remoteEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

		udpClient = new UdpClient();
		udpClient.Connect(SERVER_IP, UDP_PORT);

		udpReciever();
	}

	void initTCP()
	{
		tcpClient = new TcpClient();
		tcpClient.Connect(SERVER_IP, TCP_PORT);
		tcpStream = tcpClient.GetStream();

		tcpReciever();
	}

	async void udpReciever()
	{
		while (true)
		{
			byte[] receiveBytes = new byte[0];
			await Task.Run(() => receiveBytes = udpClient.Receive(ref remoteEndPoint));
			string message = Encoding.ASCII.GetString(receiveBytes);
			getBytesUDP += Encoding.UTF8.GetByteCount(message);

			try
			{
				processUDPMessage(message);
			}
			catch
			{
				udpProcessErrors++;
				Debug.LogWarning("UDP process error: " + message);
			}
		}
	}

	async void tcpReciever()
	{
		serverOnline = true;
		while (true)
		{
			byte[] tcpReceivedData = new byte[1024];
			int bytesRead = 0; //this might cause problems, but I don't think so

			await Task.Run(() => bytesRead = tcpStream.Read(tcpReceivedData, 0, tcpReceivedData.Length));
			string message = Encoding.UTF8.GetString(tcpReceivedData, 0, bytesRead);

			getBytesTCP += Encoding.UTF8.GetByteCount(message);

			//Debug.Log("Got TCP Message: " + message);

			//loop through messages
			string[] messages = message.Split('|');
			foreach (string finalMessage in messages)
			{
				if (finalMessage != "") //to get rid of final message
				{
					try
					{
						processTCPMessage(finalMessage);
					}
					catch
					{
						tcpProcessErrors++;
						Debug.LogWarning("TCP process error: " + finalMessage);
					}
				}
			}
		}
	}

	public void sendTCPMessage(string message)
	{
		if(serverOnline)
		{
			message += "|";
			sendBytesTCP += Encoding.UTF8.GetByteCount(message);
			byte[] tcpData = Encoding.ASCII.GetBytes(message);
			tcpStream.Write(tcpData, 0, tcpData.Length);
		}
	}

	public void sendUDPMessage(string message)
	{
		sendBytesUDP += Encoding.UTF8.GetByteCount(message);
		//load message
		byte[] udpData = Encoding.ASCII.GetBytes(message);

		//send message
		udpClient.Send(udpData, udpData.Length);
	}

	void processUDPMessage(string message)
	{
		if (message == "pong")
		{
			udpPing.text = "UDP Latency: " + (int)((Time.time - udpPingStartTime) * 1000) + "ms";
			lastGottenPingTime = Time.time;
			return;
		}

		string[] peices = message.Split('~');
		int otherClientID = int.Parse(peices[0]);
		Vector3 otherClientPos = ServerEvents.parseVector3(peices[1]);
		Quaternion otherClientRot = ServerEvents.parseQuaternion(peices[2]);
		bool showOtherClient = bool.Parse(peices[3]);
		bool clientIsSliding = bool.Parse(peices[4]);

		OtherClient otherClient = events.getOtherClientScriptByID(otherClientID);
		otherClient.setTransform(otherClientPos, otherClientRot, clientIsSliding);
		otherClient.setVisibility(showOtherClient);
	}

	void processTCPMessage(string message)
	{
		//Debug.Log("Got TCP message from server: " + message);

		if (message == "pong")
		{
			tcpPing.text = "TCP Latency: " + (int)((Time.time - tcpPingStartTime) * 1000) + "ms";
			lastGottenPingTime = Time.time;
			return;
		}

		events.rawEvent(message);
	}

	void OnApplicationQuit()
	{
		/*//close tcp
		tcpStream.Close();
		tcpClient.Close();

		//close udp
		udpClient.Close();*/ //just makes error rn
	}

	private void Update()
	{
		if(Time.time - lastGottenPingTime > maxSecondsBeforeDisconnect)
		{
			lostConnection = true;
			SceneManager.LoadScene(0);
		}
	}
}