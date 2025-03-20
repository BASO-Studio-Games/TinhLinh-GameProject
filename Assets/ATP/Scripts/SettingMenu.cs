using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject LanguagesPanel;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance.PlaySFX("ButtonUI");
    }

    public void Back()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowLanguagesPanel()
    {
        LanguagesPanel.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void StopShowLanguagesPanel()
    {
        LanguagesPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
