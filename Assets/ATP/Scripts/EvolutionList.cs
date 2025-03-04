using UnityEngine;

public class EvolutionList : MonoBehaviour
{
    public static EvolutionList main;

    [SerializeField] private GameObject evolvedAttackPrefab;
    private void Awake()
    {
        main = this;
    }
    public GameObject GetEvolvedAttackPrefab()
    {
        return evolvedAttackPrefab;
    }
}
