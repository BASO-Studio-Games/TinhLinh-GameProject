using UnityEngine;

public class SettingGameManager : MonoBehaviour
{   
    [Header("Screen:")]//----------
    [SerializeField] private int indexHomeMenuScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToHome(){
        SceneLoader loader = gameObject.GetComponentInParent<SceneLoader>();
        loader.LoadScene(indexHomeMenuScreen);
    }
}
