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
	float bestLatency;

	public static string username;

	[SerializeField] List<ServerOption> possibleServers;
	[SerializeField] int serverTimoutMS;
	[SerializeField] int currentServerVersion;
	[SerializeField] bool rejectWrongVersionServer;

	[SerializeField] TextMeshProUGUI statusText;
	[SerializeField] Button connectButton;
	[SerializeField] GameObject lostConnectionGUI;

	[Header("Browser:")]
	[SerializeField] GameObject serverContainer;
	[SerializeField] GameObject serverPrefab;
	[SerializeField] bool autoUpdateBrowser;
	[SerializeField] float browserUpdateInterval;

	[SerializeField] Button browserJoinButton;

	ServerOption currentServerInBrowser;

	private void Start()
    {
		if(Client.lostConnection)
		{
			Client.lostConnection = false;
			lostConnectionGUI.SetActive(true);
		}
		Cursor.lockState = CursorLockMode.None;

		if(autoUpdateBrowser )
		{
			InvokeRepeating("updateBrowser", 1, browserUpdateInterval);
		}
	}

	public void updateBrowser()
	{
		print("udpated browser");
		foreach(ServerOption option in possibleServers)
		{
			option.refreshInfo();
		}
	}

	public void Deactivate(GameObject objectToDeactivate)
	{
		objectToDeactivate.SetActive(false);
	}
	public void Activate(GameObject objectToActivate)
	{
		objectToActivate.SetActive(true);
	}

	private void Update()
    {
		username = SteamHandler.usernameSteam;
	}

    public void AutoJoin()
    {
		connectButton.enabled = false;
		statusText.text = "Pinging Servers...";

		bestLatency = -1;

		foreach (ServerOption option in possibleServers)
		{
			if(option.online && (option.latency < bestLatency || bestLatency == -1))
			{
				bestPort = option.port;
				bestIP = option.ip;
				bestLatency = option.latency;
			}
		}
		Invoke("ChangeScene", ((float)serverTimoutMS + 500) / 1000);
	}

	async void ChangeScene()
	{
		if (bestLatency != -1)
		{
			Debug.Log("Best Server ----> IP: " + bestIP + ", Port: " + bestPort + ", Ping: " + bestLatency);
			statusText.text = "Joining Server...";
			await Task.Delay(750);
			SceneManager.LoadScene("Main");
		}
		else
		{
			statusText.text = "No Online Servers";
			Debug.LogError("No Online Servers");
			await Task.Delay(1500);
			connectButton.enabled = true;
			statusText.text = "Try Again";
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

	public void setSelectedServer(ServerOption selectedServer)
	{
		if(currentServerInBrowser != null)
		{
			currentServerInBrowser.gameObject.GetComponent<Image>().color = new Color(0.69f, 0.69f, 0.69f);
		}
		selectedServer.gameObject.GetComponent<Image>().color = new Color(0.47f, 0.61f, 0.43f);
		currentServerInBrowser = selectedServer;

		browserJoinButton.enabled = true;
		browserJoinButton.gameObject.GetComponent<Image>().color = new Color(248, 248, 248, 255);
	}

	public void joinSelectedServer()
	{
		if(currentServerInBrowser != null && currentServerInBrowser.online)
		{
			bestPort = currentServerInBrowser.port;
			bestIP = currentServerInBrowser.ip;
			ChangeScene();
		}
	}
}
