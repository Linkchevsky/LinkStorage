using UnityEngine;

public class Storage : MonoBehaviour
{
    public static Storage Instance;

    public GameObject ClassicUnitPrefab;
    public GameObject WarriorUnitPrefab;
    [Space]
    public GameObject MainHeadquartersPrefab;
    public GameObject ElectricPolePrefab;


    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
