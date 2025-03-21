using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private SceneLoader sceneLoader;

    public void Pause()
    {
        AudioManager.Instance.PlaySFX("ButtonUI");
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        AudioManager.Instance.PlaySFX("ButtonUI");
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Home()
    {
        AudioManager.Instance.PlaySFX("ButtonUI");
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
        Time.timeScale = 1f;

        AudioManager.Instance.PlayMusic("Theme");
    }

    public void Restart()
    {
        AudioManager.Instance.PlaySFX("ButtonUI");
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
