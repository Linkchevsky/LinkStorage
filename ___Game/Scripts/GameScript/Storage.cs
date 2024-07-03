using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public static Storage Instance;

    public GameObject ClassicUnitPrefab;
    public GameObject WarriorUnitPrefab;
    [Space]
    public GameObject MainHeadquartersPrefab;
    public GameObject ElectricPolePrefab;
    public GameObject GeneratorPrefab;
    public GameObject BatteryPrefab;

    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Dictionary<Vector3, Wire> WiresDictionary = new Dictionary<Vector3, Wire>();

    public List<BuildingInterface> AllBuildingsInterface = new List<BuildingInterface>();
    public List<Collider2D> AllBuildingsColliders = new List<Collider2D>();
    public List<GameObject> AllBuildingsGO = new List<GameObject>();

    public List<UnitInterface> AllUnitsInterface = new List<UnitInterface>();
    public List<Collider2D> AllUnitsColliders = new List<Collider2D>();
    public List<GameObject> AllUnitsGO = new List<GameObject>();



    public void RemoveFromBuildingsList(int id) 
    {
        AllBuildingsInterface.RemoveAt(id); 
        AllBuildingsColliders.RemoveAt(id);
        AllBuildingsGO.RemoveAt(id);
    }

    public void RemoveFromUnitsList(int id)
    {
        AllUnitsInterface.RemoveAt(id);
        AllUnitsColliders.RemoveAt(id);
        AllUnitsGO.RemoveAt(id);
    }
}
