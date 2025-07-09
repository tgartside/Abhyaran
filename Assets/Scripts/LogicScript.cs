using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{

    [Header("Object References")]
    public GameObject pauseMenu;
    public GameObject winScreen;

    private void Start()
    {
        Application.targetFrameRate = 144;
        Time.timeScale = 1.0f;
        if (SceneManager.GetActiveScene().name.Equals("TerrainTesting"))
        {
            StartCoroutine(FadeIn());
        }
    }
    private void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("TitleScreen"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenu.activeSelf)
                {
                    Unpause();
                }

                else
                {
                    pauseMenu.SetActive(true);
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }
        }
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        StartCoroutine(FadeOut());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    IEnumerator FadeIn()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFade>().StartFadeIn();
        yield return new WaitForSeconds(1);
    }

    IEnumerator FadeOut()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFade>().StartFadeOut();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("TerrainTesting");
    }
}
