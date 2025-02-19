using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Back()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
