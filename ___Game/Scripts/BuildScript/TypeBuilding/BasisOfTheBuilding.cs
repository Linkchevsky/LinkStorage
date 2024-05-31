using Mirror;
using Pathfinding.Poly2Tri;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasisOfTheBuilding;

public class BasisOfTheBuilding : NetworkBehaviour, BuildingInterface
{
    [SerializeField] protected Collider2D _collider2D;
    [SerializeField] protected GameObject _wirePrefab;
    [Space]
    [SerializeField] public BuildingInfo _thisBuildingInfo;
    [SerializeField] public BasisOfTheBuilding _thisScriptFromInspector; //используется и как ссылка на интерфейс

    [SyncVar(hook = nameof(AfterTheEnergyChange))] public int BuildCurrentEnergy;

    public class BuildingCharacteristics
    {
        public MainHeadquarter TheMainScriptOfTheElectricalNetwork = null;
        public int NumberInTheElectricalSystem;
        public List<BuildingInterface> BuildingNeighborsInterface = new List<BuildingInterface>();

        public BuildingInfo ThisBuildingInfo;
        public BasisOfTheBuilding ThisScriptFromInspector; //используется и как ссылка на интерфейс

        public List<string> ListOfSpawnUnits = null;
        public List<string> listOfAdditionalFunctionality = null;

        public bool CanvasUsed;
        public bool InElectricalSystem;

        public int ChargingTheBuilding = 0;
    }
    protected BuildingCharacteristics _buildingCharacteristics = new BuildingCharacteristics();

    private void OnEnable() 
    {
        _buildingCharacteristics.ThisBuildingInfo = _thisBuildingInfo;
        _buildingCharacteristics.ThisScriptFromInspector = _thisScriptFromInspector;

        GlobalUpdate.s_energyTick += EnergyTick;

        Storage.Instance.AllBuildingsGO.Add(this.gameObject);
        Storage.Instance.AllBuildingsInterface.Add(this);
        Storage.Instance.AllBuildingsColliders.Add(_collider2D);

        CheckingElectricalNetwork();
    }
    private void OnDisable() => GlobalUpdate.s_energyTick -= EnergyTick;

    private void EnergyTick()
    {
        if (BuildCurrentEnergy > 0)
            UsedEnergy(-1);

        if (_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ChargingPower != 0)
        {
            if (BuildCurrentEnergy <= _buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy)
            {
                UsedEnergy(_buildingCharacteristics.ChargingTheBuilding);
            }
            else
                UsedEnergy(_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.BuildCurrentEnergy);
        }
    }


    public void CheckingElectricalNetwork()
    {
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, 5, LayerMask.GetMask("Obstacle")).ToList();
        colliders.Remove(_collider2D);

        if (colliders.Count != 0)
        {
            _buildingCharacteristics.BuildingNeighborsInterface.Clear();

            List<Collider2D> notAddedInList = new List<Collider2D>();
            List<Collider2D> addedInList = new List<Collider2D>();
            for (int i = 0; i < colliders.Count; i++)
            {
                BuildingInterface colliderInterface = Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsColliders.IndexOf(colliders[i])];

                MainHeadquarter mainGOScript;
                try
                {
                    mainGOScript = colliderInterface.GetBuildingCharacteristics().TheMainScriptOfTheElectricalNetwork;
                }
                catch(ArgumentException)
                {
                    continue;
                }

                _buildingCharacteristics.BuildingNeighborsInterface.Add(colliderInterface);

                if (mainGOScript != null)
                {
                    addedInList.Add(colliders[i]);

                    if (_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
                    {
                        _buildingCharacteristics.InElectricalSystem = true;
                        _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork = mainGOScript;
                        _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.AddInElectricalSystemList(_buildingCharacteristics.ThisScriptFromInspector);
                        _buildingCharacteristics.NumberInTheElectricalSystem = _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count - 1;
                    }
                }
                else
                    notAddedInList.Add(colliders[i]);
            }
             
            List<GameObject> listOfBuildingsGO = new List<GameObject>();
            foreach (Collider2D collider in addedInList)
                listOfBuildingsGO.Add(collider.gameObject);
            InstallationOfWires(listOfBuildingsGO);

            CheckingTheNeighbors(notAddedInList);
        }
    }

    public void InstallationOfWires(List<GameObject> listOfBuildingsGO)
    {
        for (int i = 0; i < listOfBuildingsGO.Count; i++)
        {
            Wire line = Instantiate(_wirePrefab, transform.GetChild(0)).GetComponent<Wire>();
            line.buildingsNumbers = new int[2] { Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsGO.IndexOf(transform.gameObject)].GetBuildingCharacteristics().NumberInTheElectricalSystem, 
                Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsGO.IndexOf(listOfBuildingsGO[i])].GetBuildingCharacteristics().NumberInTheElectricalSystem }; //получение номеров зданий

            _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.AllWiresList.Add(line);

            Vector3 dir = listOfBuildingsGO[i].transform.position - transform.position;

            line.transform.position = transform.position + dir / 2;
            line.transform.localScale = new Vector3(dir.magnitude, line.transform.localScale.y, line.transform.localScale.z);
            line.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

    }

    private void CheckingTheNeighbors(List<Collider2D> notAddedInList)
    {
        foreach (Collider2D collider in notAddedInList)
            Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsColliders.IndexOf(collider)].CheckingElectricalNetwork();
    }



    public void Interaction() 
    {
        _buildingCharacteristics.CanvasUsed = true;
        CanvasControl.Instance.deselectFromCanvas += Deselect;

        CanvasControl.Instance.UsingCanvas(_buildingCharacteristics.ThisBuildingInfo.Id, $"{BuildCurrentEnergy}/" +
            $"{_buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy}", null, _buildingCharacteristics.listOfAdditionalFunctionality, _buildingCharacteristics.ThisScriptFromInspector); 
    }
    public void Deselect()
    {
        _buildingCharacteristics.CanvasUsed = false;
        CanvasControl.Instance.deselectFromCanvas -= Deselect;
    }

    public void UsedEnergy(int amountOfEnergy)
    {
        if (isOwned)
            CmdUsedEnergy(amountOfEnergy);
    }
    [Command]
    private void CmdUsedEnergy(int amountOfEnergy) => BuildCurrentEnergy += amountOfEnergy;

    private void AfterTheEnergyChange(int oldValue, int newValue)
    {
        if (_buildingCharacteristics.CanvasUsed)
            CanvasControl.Instance.EnergyChangeAction?.Invoke($"{BuildCurrentEnergy}/{_buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy}");
    }



    public GameObject GetGameobject() { return this.gameObject; }
    public Collider2D GetBoxCollider() { return _collider2D; }
    public BuildingCharacteristics GetBuildingCharacteristics() { return _buildingCharacteristics; }
    public void SetBuildingChargingPower(int power) 
    { 
        if (_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.FreeChargingPower - power >= 0) 
        {
            _buildingCharacteristics.ChargingTheBuilding += power;
            _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.FreeChargingPower -= power; 
        } 
    }
    public int GetEnergy() { return BuildCurrentEnergy; }
}