using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private SceneLoader sceneLoader;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Home()
    {
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene(SceneManager.GetActiveScene().name);
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
