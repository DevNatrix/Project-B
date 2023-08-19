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

	public List<string> verbs;
	public List<string> adverbs;

	public void newFeed(string killer, string killed, int verbIndex, int adverbIndex)
	{
		//create kill feed child
		TextMeshProUGUI newChild = Instantiate(killFeedPrefab, killFeedParent).GetComponent<TextMeshProUGUI>();

		//set message
		newChild.text = killer + " " + verbs[verbIndex] + " " + killed + " " + adverbs[adverbIndex];

		//destroy
		Destroy(newChild.gameObject, killFeedLife);

	}

	public void createNewFeed(string killer, string killed, int verbIndex = -1, int adverbIndex = -1)
	{
		if (verbIndex == -1)
		{
			verbIndex = Random.Range(0, verbs.Count);
		}
		
		if (adverbIndex == -1)
		{
			adverbIndex = Random.Range(0, adverbs.Count);
		}

		serverEvents.sendGlobalEvent("newKillFeed", new string[] { killer, killed, verbIndex + "", adverbIndex + "" });
	}
}
