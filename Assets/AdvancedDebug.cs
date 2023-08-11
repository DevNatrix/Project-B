using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdvancedDebug : MonoBehaviour
{
	public GameObject debugParent;
	public GameObject debugChildPrefab;

	public TextMeshProUGUI createDebug(string debugName)
	{
		GameObject newDebug = GameObject.Instantiate(debugChildPrefab, debugParent.transform);
		newDebug.name = debugName;
		TextMeshProUGUI newDebugTMP = newDebug.GetComponent<TextMeshProUGUI>();
		newDebugTMP.text = debugName;
		return newDebugTMP;
	}
}
