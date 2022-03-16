using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public bool GameIsPaused;
    public static MenuManager Instance;
    public GameObject pauseMenuUI, standardUI, failGameUI, failCamera, gameCamera;
    public AudioSource backgroundMusic;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        failGameUI.SetActive(false);
        standardUI.SetActive(true);
        GameIsPaused = false;
        failCamera.SetActive(false);
        Time.timeScale = 1f;
    }

#if true
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
#endif

    public void PauseGame()
    {
        standardUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void ContinueGame()
    {
        standardUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void RestartGame()
    {
        backgroundMusic.Play();
        Debug.ClearDeveloperConsole();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ContinueGame();
    }

    public void NextLevel2()
    {
        Debug.ClearDeveloperConsole();
        SceneManager.LoadScene("Level2 - Ring");
    }

    public void NextLevel3()
    {
        Debug.ClearDeveloperConsole();
        SceneManager.LoadScene("Level3 - Snow");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("RadMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
