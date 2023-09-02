using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerListItemUI : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI pingText;

    public void setStartInfo(string username)
    {
        usernameText.text = username;
    }

	public void updatePing(int ping)
	{
		pingText.text = ping + "";
	}
}
