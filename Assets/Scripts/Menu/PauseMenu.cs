using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private SceneLoader sceneLoader;

    public void Pause()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance null!");
            return;
        }

        AudioManager.Instance.PlaySFX("ButtonUI");
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance null!");
            return;
        }

        AudioManager.Instance.PlaySFX("ButtonUI");
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Home()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance null!");
            return;
        }

        AudioManager.Instance.PlaySFX("ButtonUI");
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
        Time.timeScale = 1f;

        AudioManager.Instance.PlayMusic("Theme");
    }

    public void Restart()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance null!");
            return;
        }

        AudioManager.Instance.PlaySFX("ButtonUI");
        SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
        loader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
