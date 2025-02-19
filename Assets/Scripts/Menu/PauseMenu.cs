using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    
    void Start()
    {
        pauseButton.onClick.AddListener(OnClickPauseButton);
        resumeButton.onClick.AddListener(OnClickResumeButton);
        restartButton.onClick.AddListener(OnClickRestartButton);
        homeButton.onClick.AddListener(OnClickHomeButton);
    }

    private void OnClickPauseButton(){
        Debug.Log("Pause Button Clicked");
        pauseMenu.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    private void OnClickResumeButton(){
        Debug.Log("Resume Button Clicked");
        pauseMenu.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        Time.timeScale = 1;
    }

    private void OnClickRestartButton(){
        Time.timeScale = 1;
        Debug.Log("Restart Button Clicked: " + SceneManager.GetActiveScene().name);
        SceneLoader loader = gameObject.GetComponent<SceneLoader>();
        loader.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnClickHomeButton(){
        Time.timeScale = 1;
        Debug.Log("Home Button Clicked");
        SceneLoader loader = gameObject.GetComponent<SceneLoader>();
        loader.LoadScene("Home Menu");
    }
}
