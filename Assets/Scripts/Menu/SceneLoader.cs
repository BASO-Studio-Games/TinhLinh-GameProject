using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject LoaderUI;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;
 
    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneCoroutine(index));
    }
 
    private IEnumerator LoadSceneCoroutine(int index)
    {
        progressSlider.value = 0;
        LoaderUI.SetActive(true);
 
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;
 
        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                asyncOperation.allowSceneActivation = true;
            }
            progressText.text = Mathf.RoundToInt(progressSlider.value * 100).ToString();
            yield return null;
        }
    }
}