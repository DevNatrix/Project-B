using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static int rounds = 10;
	public static int matches = 3;
	public static float matchTimer = 0;
	public static float matchTimerStart = 2;
	public static float team0MatchCount = 0;
	public static float team1MatchCount = 0;
	public static float team0RoundCount = 0;
	public static float team1RoundCount = 0;
	public static bool matchInProgress = false;
	public static bool dead = false;

	[SerializeField] ServerEvents serverEvents;

	public void checkMatchStatus()
	{
		int aliveTeam = -1;
		if (!GameManager.dead)
		{
			aliveTeam = PlayerManager.team;
		}

		foreach (OtherClient otherClient in serverEvents.otherClientList)
		{
			if (!otherClient.dead)
			{
				if (aliveTeam == -1)
				{
					aliveTeam = otherClient.team;
				}
				else if (aliveTeam != otherClient.team)
				{
					return;
				}
			}
		}

		if(aliveTeam == 0)
		{
			team0MatchCount++;
		}
		else
		{
			team1MatchCount++;
		}
		serverEvents.sendGlobalEvent("teamWonMatch", new string[] { aliveTeam + "" });
		
		if(team0MatchCount >= matches || team1MatchCount >= matches) //if a team won the matches
		{
			if(aliveTeam == 0)
			{
				team0RoundCount++;
			}
			else { 
				team1RoundCount++; 
			}

			serverEvents.sendGlobalEvent("teamWonRound", new string[] { aliveTeam + ""});
			serverEvents.sendGlobalEvent("startRound", new string[] { matches + "", team0RoundCount + "", team1RoundCount + "" });

		}
		serverEvents.sendGlobalEvent("startMatch", new string[] { matchTimerStart + "", team0MatchCount + "", team1MatchCount + "" });
		matchInProgress = false;
	}
}
