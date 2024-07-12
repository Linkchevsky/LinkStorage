using Mirror;
using Pathfinding.Poly2Tri;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasisOfTheBuilding;
using static UnityEngine.Rendering.DebugUI;

public class BasisOfTheBuilding : NetworkBehaviour, BuildingInterface
{
    [SerializeField] protected Collider2D _collider2D;
    [SerializeField] protected GameObject _wirePrefab;
    [Space]
    [SerializeField] public BuildingInfo _thisBuildingInfo;
    [SerializeField] public BasisOfTheBuilding _thisScriptFromInspector; //������������ � ��� ������ �� ���������

    [SyncVar(hook = nameof(AfterTheEnergyChange))] public int BuildCurrentEnergy;

    #region[������]
    public class BuildingCharacteristicsClass
    {
        public MainHeadquarter TheMainScriptOfTheElectricalNetwork = null;
        public int NumberInTheElectricalSystem;

        public BuildingInfo ThisBuildingInfo;
        public BasisOfTheBuilding ThisScriptFromInspector; //������������ � ��� ������ �� ���������

        public List<string> ListOfSpawnUnits = null;
        public List<string> listOfAdditionalFunctionality = null;

        public bool CanvasUsed;
        public bool InElectricalSystem;

        public int ChargingPowerTheBuilding = 0;

        public List<List<Vector3>> EnergyPaths = new List<List<Vector3>>();
    }
    public BuildingCharacteristicsClass BuildingCharacteristics = new BuildingCharacteristicsClass();
    #endregion

    private void OnEnable() 
    {
        Storage.Instance.AllBuildingsGO.Add(this.gameObject);
        Storage.Instance.AllBuildingsInterface.Add(this);
        Storage.Instance.AllBuildingsColliders.Add(_collider2D);

        BuildingCharacteristics.ThisBuildingInfo = _thisBuildingInfo;
        BuildingCharacteristics.ThisScriptFromInspector = _thisScriptFromInspector;

        GlobalUpdate.s_energyTick += EnergyTick;
    }
    private void OnDisable() => GlobalUpdate.s_energyTick -= EnergyTick;

    private void EnergyTick()
    {
        if (BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
            return;

        if (BuildCurrentEnergy > 0 )
            UsedEnergy(-1);

        if (BuildingCharacteristics.ChargingPowerTheBuilding != 0 && BuildCurrentEnergy < BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy)
            UsedEnergy(BuildingCharacteristics.ChargingPowerTheBuilding);
    }


    public void CheckingElectricalNetwork()
    {
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, 5, LayerMask.GetMask("Obstacle")).ToList();
        colliders.Remove(_collider2D);

        if (colliders.Count != 0)
        {
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
                catch
                {
                    continue;
                }

                if (mainGOScript != null)
                {
                    addedInList.Add(colliders[i]);

                    if (BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
                    {
                        BuildingCharacteristics.InElectricalSystem = true;
                        BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork = mainGOScript;
                    }
                }
                else
                    notAddedInList.Add(colliders[i]);
            }
             
            List<GameObject> listOfBuildingsGO = new List<GameObject>();
            foreach (Collider2D collider in addedInList)
                listOfBuildingsGO.Add(collider.gameObject);

            InstallationOfWires(listOfBuildingsGO, notAddedInList);
        }
    }

    public void InstallationOfWires(List<GameObject> listOfBuildingsGO, List<Collider2D> notAddedInList = null)
    {
        for (int i = 0; i < listOfBuildingsGO.Count; i++)
        {
            Wire wireScript = Instantiate(_wirePrefab, Storage.Instance.transform).GetComponent<Wire>();

            Vector3 dir = listOfBuildingsGO[i].transform.position - transform.position;

            wireScript.transform.position = transform.position + dir / 2;
            Vector3 wirePosition = new Vector3((float)Math.Round(wireScript.transform.position.x * 100) / 100, (float)Math.Round(wireScript.transform.position.y * 100) / 100, 0);
            wireScript.transform.position = wirePosition;

            wireScript.transform.localScale = new Vector3(dir.magnitude, wireScript.transform.localScale.y, wireScript.transform.localScale.z);
            wireScript.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            Storage.Instance.WiresDictionary.Add(wirePosition, wireScript);
        }

        if (notAddedInList != null)
            CheckingTheNeighbors(notAddedInList);
    }

    private void CheckingTheNeighbors(List<Collider2D> notAddedInList)
    {
        foreach (Collider2D collider in notAddedInList)
            Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsColliders.IndexOf(collider)].CheckingElectricalNetwork();
    }



    public void Interaction() 
    {
        BuildingCharacteristics.CanvasUsed = true;
        CanvasControl.Instance.deselectFromCanvas += Deselect;

        CanvasControl.Instance.UsingCanvas(BuildingCharacteristics.ThisBuildingInfo.Id, $"{BuildCurrentEnergy}/" +
            $"{BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy}", null, BuildingCharacteristics.listOfAdditionalFunctionality, BuildingCharacteristics.ThisScriptFromInspector); 
    }
    public void Deselect()
    {
        BuildingCharacteristics.CanvasUsed = false;
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
        if (BuildingCharacteristics.CanvasUsed)
            CanvasControl.Instance.EnergyChangeAction?.Invoke($"{BuildCurrentEnergy}/{BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy}");
    }



    public GameObject GetGameobject() { return this.gameObject; }
    public Collider2D GetBoxCollider() { return _collider2D; }
    public BuildingCharacteristicsClass GetBuildingCharacteristics() { return BuildingCharacteristics; }
    public void SetBuildingChargingPower(int power) 
    { 
        if (BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.FreeChargingPower - power >= 0) 
        {
            BuildingCharacteristics.ChargingPowerTheBuilding += power;
            BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.FreeChargingPower -= power; 
        } 
    }
    public int GetEnergy() { return BuildCurrentEnergy; }
}