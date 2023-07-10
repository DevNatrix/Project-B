using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreboardController : MonoBehaviour
{
    PlayerControls playerControls;
    public GameObject ScoreBoardGO;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerControls.UI.Scoreboard.WasPressedThisFrame() && !ScoreBoardGO)
        {
            ScoreBoardGO.SetActive(true);
        }
        else if(playerControls.UI.Scoreboard.WasReleasedThisFrame() && ScoreBoardGO)
        {
            ScoreBoardGO.SetActive(false);
        }
    }
}
