using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Console;
using RetroAesthetics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ParticleSystem ps, speedBoostPS;
    public Text scoreText;

    GameObject[] getAmpsTotal;
    GameObject[] getSpeedBoosterTotal;
    GameObject[] getSlowTimeBoosterTotal;
    private int score = 0;
    private int totalScoreOnSurface;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //don't close phone while playing
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ps.Stop();
        speedBoostPS.Stop();
        totalScoreOnSurface = GetTotalScoreFromSurface();
    }

    // Update is called once per frame
    void Update()
    {   //Restarts the game whenever R key is pressed, but only if the PauseMenu(a.k.a MenuManager) is NOT active
        if (Input.GetKeyDown(KeyCode.R) && !MenuManager.Instance.GameIsPaused)
        {
            Debug.ClearDeveloperConsole();
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("Level1 - Baby", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Level2 - Ring", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("Level3 - Snow", LoadSceneMode.Single);
        }

        enableSpeedParticle();
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = ((int)score).ToString();
    }

    public int GetScore()
    {
        return score;
    }

    private int GetTotalScoreFromSurface()
    {
        int scoreForOneAmps = PickUpManager.Instance.ForAmps;
        int scoreForOneSlowTimeBooster = PickUpManager.Instance.ForSlowTimeBooster;
        int scoreForOneSpeedBooster = PickUpManager.Instance.ForSpeedBooster;

        getAmpsTotal = GameObject.FindGameObjectsWithTag("PickUpStatic");
        getSpeedBoosterTotal = GameObject.FindGameObjectsWithTag("PickUpSpeedBoost");
        getSlowTimeBoosterTotal = GameObject.FindGameObjectsWithTag("SlowTimeObject");

        int totalNumberOfAmpsSpawns = getAmpsTotal.Length;
        int totalNumberOfSlowTimeSpawns = getSlowTimeBoosterTotal.Length;
        int totalNumberOfSpeedTimeSpawns = getSpeedBoosterTotal.Length;

        int totalPointsForAmps = scoreForOneAmps * totalNumberOfAmpsSpawns;
        int totalPointsForSlowTimeBooster = scoreForOneSlowTimeBooster * totalNumberOfSlowTimeSpawns;
        int totalPointsForSpeedBooster = scoreForOneSpeedBooster * totalNumberOfSpeedTimeSpawns;

        int totalPointsOnSurface = totalPointsForAmps + totalPointsForSlowTimeBooster + totalPointsForSpeedBooster;

        return totalPointsOnSurface;
    }

    public float AwardPlayer()
    {
        float getTotalScoreFromSruface = totalScoreOnSurface;
        float totalPlayerScore = score;

        float x = totalPlayerScore * 100 / getTotalScoreFromSruface;
        Debug.Log(x);
        if (x == 100f || totalPlayerScore == getTotalScoreFromSruface)
        {
            Debug.Log("PERFECT");
            return 10f;
        }

        else if (x >= 80f && x <= 99f)
        {
            Debug.Log("3 STARS");
            return 3f;
        }

        else if (x >= 50f && x <= 79f)
        {
            Debug.Log("2 STARS");
            return 2f;
        }

        else if (x >= 1f && x <= 49f)
        {
            Debug.Log("1 STAR");
            return 1f;
        }

        return 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void levelOne()
    {
        SceneManager.LoadScene("RingFantasyScene", LoadSceneMode.Single);
    }

    public void levelTwo()
    {
        SceneManager.LoadScene("MountainFantasyScene", LoadSceneMode.Single);
    }

    public void levelThree()
    {
        SceneManager.LoadScene("RingFantasyScene - Baby edition", LoadSceneMode.Single);
    }

    private void enableSpeedParticle()
    {
        if (PlayerController.Instance.speed <= 15.0f)
        {
            ps.Stop();
        }
        else
        {
            ps.Play();
        }
    }

    void SaveGame() { } //TODO: Implement Save Game
}
