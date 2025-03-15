using UnityEngine;
using UnityEngine.SceneManagement;

public class WInOrLoseMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string nextMapName;
    
    void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void Next()
    {
        if (nextMapName != null){
            SceneLoader loader = sceneLoader.GetComponent<SceneLoader>();
            loader.LoadScene(nextMapName);
            Time.timeScale = 1f;
        }
        else
        {
            Home();
        }
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

