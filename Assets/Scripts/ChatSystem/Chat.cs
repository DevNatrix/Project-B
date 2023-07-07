using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    public GameObject ChatBackground;
    public TMP_InputField InputFieldContainer;

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
        if (Input.GetKeyDown(KeyCode.Return) && ChatBackground.activeSelf == false)
        {
            ChatBackground.SetActive(true);
            InputFieldContainer.ActivateInputField();
            //To do: disable all inputs such as movement, firing etc.
        }
        //If nothing is typed and client has clicked enter it disables ChatBackground
        else if (ChatBackground.activeSelf == true && string.IsNullOrWhiteSpace(ChatContainer) && Input.GetKeyDown(KeyCode.Return))
        {
            ChatBackground.SetActive(false);
        }
        //If client has typed something and pressed enter it'll send message through the network and disables ChatBackground
        else if (ChatBackground.activeSelf == true && !string.IsNullOrWhiteSpace(ChatContainer) && Input.GetKeyDown(KeyCode.Return))
        {
            //Send Message(Joe code this)
            ChatBackground.SetActive(false);
        }
    }
}
