using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollButtonEffect : MonoBehaviour
{
    [SerializeField] private Button rollButton;
    [SerializeField] private Animator buttonAnimator;


    private void Update()
    {
        if (LevelManager.main.isMaxEnergy)
        {
            rollButton.gameObject.SetActive(true);
            rollButton.interactable = true;
            buttonAnimator.SetBool("isReady", true);
        }
        else
        {
            rollButton.gameObject.SetActive(false);
            buttonAnimator.SetBool("isReady", false);
        }
    }
}
