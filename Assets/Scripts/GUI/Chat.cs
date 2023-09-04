using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class Chat : MonoBehaviour
{
    public GameObject ChatBackground;
    public TMP_InputField InputFieldContainer;
    public GameObject InputFieldContainerGO;
	[SerializeField] Transform chatMessagesContainer;
	[SerializeField] GameObject chatMessagePrefab;
	[SerializeField] ServerEvents serverEvents;
	[SerializeField] PlayerManager playerManager;

	public string commandChar = "/";

	PlayerControls playerControls;

	[SerializeField] float chatTimer;
	[SerializeField] float timeBeforeClose = 2;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        //Just for security reasons yikes
        ChatBackground.SetActive(false);
    }

    private void Update()
    {
		chatTimer -= Time.deltaTime;

        //String needed to see if client typed something in chat
        string ChatContainer = InputFieldContainer.text;

        //Get input to enable ChatBackground
        if (playerControls.ChatVoice.Chat.WasPressedThisFrame() && ChatBackground.activeSelf == false)
        {
			MenuController.typing = true;
			ChatBackground.SetActive(true);
            InputFieldContainer.ActivateInputField();
        }
        //If nothing is typed and client has clicked enter it disables ChatBackground
        else if (ChatBackground.activeSelf == true && string.IsNullOrWhiteSpace(ChatContainer) && playerControls.ChatVoice.Chat.WasPressedThisFrame())
        {
			if(MenuController.typing == false) //start chatting again while it's open
			{
				MenuController.typing = true;
			}
			else //close chat
			{
				MenuController.typing = false;
				ChatBackground.SetActive(false);
			}
		}
        //If client has typed something and pressed enter it'll send message through the network and disables ChatBackground
        else if (ChatBackground.activeSelf == true && !string.IsNullOrWhiteSpace(ChatContainer) && playerControls.ChatVoice.Chat.WasPressedThisFrame())
        {
			sendMessage(ChatContainer);
            InputFieldContainer.text = "";
			chatTimer = timeBeforeClose;

			MenuController.typing = false;
        }

		if(MenuController.typing || chatTimer > 0)
		{
			if(!ChatBackground.activeSelf)
			{
				ChatBackground.SetActive(true);
				//InputFieldContainer.ActivateInputField();
			}
		}
		else
		{
			ChatBackground.SetActive(false);
		}
    }

	public void sendMessage(string message)
	{
		if(message.Substring(0, 1) != commandChar)
		{
			serverEvents.sendGlobalEvent("chatMessage", new string[] { Lobby.username, message });
		}
		else//process commands
		{
			string command = message.Substring(1, message.Length - 1);
			
			if(command == "die")
			{
				playerManager.SetHealth(0);
				serverMessage("ok lolz");
			}
			else
			{
				serverMessage("Err: Uknown command");
			}
		}

	}

	public void newMessage(string username, string message)
	{
		GameObject messageObject = Instantiate(chatMessagePrefab, chatMessagesContainer);
		TextMeshProUGUI textObject = messageObject.GetComponent<TextMeshProUGUI>();
		textObject.text = username + ": " + message;

		chatTimer = timeBeforeClose;
	}

	public void serverMessage(string message)
	{
		GameObject messageObject = Instantiate(chatMessagePrefab, chatMessagesContainer);
		TextMeshProUGUI textObject = messageObject.GetComponent<TextMeshProUGUI>();
		textObject.color = Color.red;
		textObject.text = message;

		chatTimer = timeBeforeClose;
	}
}
