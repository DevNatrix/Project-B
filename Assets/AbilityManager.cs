using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
	public static AbilityManager Instance;

	[HideInInspector] public List<float> multipliers;
	public List<float> steps;
	public List<string> multiplierNames;
	public List<TextMeshProUGUI> stepInfoText;
	public List<Button> multitplierButtons;
	public int upgradesToChooseFrom;

	private void Start()
	{
		Instance = this;

		//set up multipliers list
		multipliers = new List<float>();
		foreach(float step in steps)
		{
			multipliers.Add(1);
		}
		Debug.Log(multipliers.Count);

		resetUpgrades();
		updateKillMenu();
	}

	public void updateKillMenu()
	{
		for(int index = 0; index < multipliers.Count; index++)
		{
			stepInfoText[index].text = (multipliers[index] - 1) * 100 + "% + " + steps[index] * 100 + "%";
		}
	}

	public void chooseSelectable()
	{
		foreach(Button button in multitplierButtons)
		{
			button.interactable = false;
		}

		for(int i = 0; i < upgradesToChooseFrom; i++)
		{
			int index = Random.Range(0, multipliers.Count - 1);
			if (multitplierButtons[index].interactable)
			{
				i--;
			}
			else
			{
				multitplierButtons[index].interactable = true;
			}
		}
	}

	public void step(TextMeshProUGUI callerText)
	{
		for (int index = 0; index < multipliers.Count; index++)
		{
			if (stepInfoText[index] == callerText)
			{
				multipliers[index] += steps[index];
				updateKillMenu();
			}
		}
	}

	public void resetUpgrades()
	{
		for (int index = 0; index < multipliers.Count; index++)
		{
			multipliers[index] = 1;
		}
	}

	public float getMultiplier(string name)
	{
		for (int index = 0; index < multipliers.Count; index++)
		{
			if (multiplierNames[index] == name)
			{
				return multipliers[index];
			}
		}
		Debug.LogError("Couldnt find multiplier " + name);
		return 1;
	}
}
