using UnityEngine;

public class EvolutionList : MonoBehaviour
{
    public static EvolutionList main;

    [SerializeField] private GameObject evolvedAttackPrefab;
    [SerializeField] private GameObject evolvedBompPrefab;
    [SerializeField] private GameObject evolvedFanPrefab;
    private void Awake()
    {
        main = this;
    }
    public GameObject GetEvolvedAttackPrefab()
    {
        return evolvedAttackPrefab;
    }

    public GameObject GetEvolvedBompPrefab()
    {
        return evolvedBompPrefab;
    }

    public GameObject GetEvolvedFanPrefab()
    {
        return evolvedFanPrefab;
    }
}
