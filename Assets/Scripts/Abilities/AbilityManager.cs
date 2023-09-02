using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
	public static AbilityManager Instance;

	[HideInInspector] public List<float> multipliers;
	List<float> steps;
	List<float> maxMultpliers; //not actually doing anything yet 

	public int upgradesToChooseFrom;
	public List<string> multiplierNames;
	public List<TextMeshProUGUI> stepInfoText;
	public List<Button> multitplierButtons;
	[SerializeField] PlayerManager playerManager;
	[SerializeField] MenuController menuController;

	public float defaultStep = 0.1f;
	public float defaultMaxMult = 999f;



	public void newSettings(string[] settings)
	{
		int ability = int.Parse(settings[1]);

		steps[ability] = float.Parse(settings[2]);
		maxMultpliers[ability] = float.Parse(settings[3]);
		return;
	}

	private void Start()
	{
		Instance = this;

		//set up multipliers list
		multipliers = new List<float>();
		steps = new List<float>();
		maxMultpliers = new List<float>();
		foreach(string multiplier in multiplierNames)
		{
			multipliers.Add(1);
			steps.Add(defaultStep);
			maxMultpliers.Add(defaultMaxMult);
		}

		resetUpgrades();
		updateKillMenu();
	}

	public void updateKillMenu()
	{
		for(int index = 0; index < multipliers.Count; index++)
		{
			stepInfoText[index].text = Mathf.Round(((multipliers[index] - 1) * 100)) + "% + " + Mathf.Round((steps[index] * 100)) + "%";
		}
	}

	public void chooseSelectable()
	{
		foreach(Button button in multitplierButtons)
		{
			button.interactable = false;
		}

		int missed = 0;
		for (int i = 0; i - missed < upgradesToChooseFrom; i++)
		{
			int index = Random.Range(0, multipliers.Count - 1);
			if (multitplierButtons[index].interactable || multipliers[index] >= maxMultpliers[index])
			{
				missed++;
				if(missed > 100) //just in case most abilities are maxed
				{
					Debug.LogWarning("Ended selecting multipliers early: not enough");
					return;
				}
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
		menuController.setDeathMenu(false);
	}

	public void resetUpgrades()
	{
		for (int index = 0; index < multipliers.Count; index++)
		{
			multipliers[index] = 1;
		}

		updateKillMenu();
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
