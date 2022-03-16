using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour {

    public bool EndGameUIIsActive;
    public static EndGameUI Instance;
    public GameObject endGameMenu, standardUI, oneStarUI, twoStarsUI, threeStarsUI, perfectText;
    public Text scoreText;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        endGameMenu.SetActive(false);
        oneStarUI.SetActive(false);
        twoStarsUI.SetActive(false);
        threeStarsUI.SetActive(false);
        perfectText.SetActive(false);
        EndGameUIIsActive = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerEndGame(float point)
    {
        if (point == 10f)
        {
            perfectText.SetActive(true);
        }
        else if (point == 3f)
        {
            threeStarsUI.SetActive(true);
        }
        else if (point == 2f)
        {
            twoStarsUI.SetActive(true);
        }
        else if (point == 1f)
        {
            oneStarUI.SetActive(true);
        }
        else if (point == 0f)
        {
            oneStarUI.SetActive(true);
        }

        standardUI.SetActive(false);
        endGameMenu.SetActive(true);
        scoreText.text = GameManager.Instance.scoreText.text;
    }
}
