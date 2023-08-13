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

    PlayerControls playerControls;

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
        //String needed to see if client typed something in chat
        string ChatContainer = InputFieldContainer.text;

        //Get input to enable ChatBackground
        if (playerControls.ChatVoice.Chat.WasPressedThisFrame() && ChatBackground.activeSelf == false)
        {
            ChatBackground.SetActive(true);
            InputFieldContainer.ActivateInputField();
            //To do: disable all inputs such as movement, firing etc.
        }
        //If nothing is typed and client has clicked enter it disables ChatBackground
        else if (ChatBackground.activeSelf == true && string.IsNullOrWhiteSpace(ChatContainer) && playerControls.ChatVoice.Chat.WasPressedThisFrame())
        {
            ChatBackground.SetActive(false);
        }
        //If client has typed something and pressed enter it'll send message through the network and disables ChatBackground
        else if (ChatBackground.activeSelf == true && !string.IsNullOrWhiteSpace(ChatContainer) && playerControls.ChatVoice.Chat.WasPressedThisFrame())
        {
            serverEvents.sendGlobalEvent("chatMessage", new string[] { Lobby.username, ChatContainer });
            InputFieldContainer.text = "";
        }
    }

	public void newMessage(string username, string message)
	{
		GameObject messageObject = Instantiate(chatMessagePrefab, chatMessagesContainer);
		TextMeshProUGUI textObject = messageObject.GetComponent<TextMeshProUGUI>();
		textObject.text = username + ": " + message;

        Debug.Log("Message Recieved Or Sent");
	}

	public void serverMessage(string message)
	{
		GameObject messageObject = Instantiate(chatMessagePrefab, chatMessagesContainer);
		TextMeshProUGUI textObject = messageObject.GetComponent<TextMeshProUGUI>();
		textObject.color = Color.red;
		textObject.text = message;
	}
}
