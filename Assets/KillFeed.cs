using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
	[HideInInspector] public static KillFeed Instance;

	[SerializeField] GameObject killFeedPrefab;
	[SerializeField] Transform killFeedParent;
	[SerializeField] float killFeedLife;
	[SerializeField] ServerEvents serverEvents;

	public List<string> waysToKill;

	public void newFeed(string killer, string killed, int wayToKillIndex = -1)
	{
		//create kill feed child
		TextMeshProUGUI newChild = Instantiate(killFeedPrefab, killFeedParent).GetComponent<TextMeshProUGUI>();

		//set message
		newChild.text = killer + " " + waysToKill[wayToKillIndex] + " " + killed;

		//destroy
		Destroy(newChild.gameObject, killFeedLife);

	}

	public void createNewFeed(string killer, string killed, int wayToKillIndex = -1)
	{
		if (wayToKillIndex == -1)
		{
			wayToKillIndex = Random.Range(0, waysToKill.Count);
		}

		serverEvents.sendGlobalEvent("newKillFeed", new string[] { killer, killed, wayToKillIndex + "" });
	}
}
