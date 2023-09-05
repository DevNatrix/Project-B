using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfo : MonoBehaviour
{
    public List<UpgradeInfo> nextUpgrades;
	public TextMeshProUGUI label;
	public Button button;
	public string title;
	bool choosable;
	public bool canChooseOnStart = false;

	public Color lockedColor;
	public Color usedColor;

	private void Start()
	{
		label.text = title;
		setChoosable(canChooseOnStart);
	}

	public void chooseUpgrade()
	{
		if(GameManager.points <= 0)
		{
			return;
		}

		Upgrades.instance.changePoints(-1);

		foreach(UpgradeInfo upgrade in nextUpgrades)
		{
			upgrade.setChoosable(true);
		}

		Upgrades.instance.chooseUpgrade(this);

		setChoosable(false);

		ColorBlock colors = button.colors;
		colors.disabledColor = usedColor;
		button.colors = colors;
	}

	public void setChoosable(bool _choosable)
	{
		choosable = _choosable;
		button.interactable = _choosable;
	}

	public void resetUpgrade()
	{
		setChoosable(canChooseOnStart);

		ColorBlock colors = button.colors;
		colors.disabledColor = lockedColor;
		button.colors = colors;
	}
}
