using Mirror;
using Pathfinding.Poly2Tri;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasisOfTheBuilding : NetworkBehaviour, BuildingInterface
{
    [SerializeField] protected Collider2D _collider2D;
    [SerializeField] protected GameObject _linePrefab;

    protected MainHeadquarter _theMainScriptOfTheElectricalNetwork = null;
    protected int _numberInTheElectricalSystem;
    protected List<BuildingInterface> _buildingNeighborsInterface = new List<BuildingInterface>();

    [SerializeField] protected BuildingInfo _thisBuildingInfo;
    [SyncVar(hook = nameof(AfterTheEnergyChange))] public int BuildCurrentEnergy;
    [SerializeField] protected BasisOfTheBuilding _thisScriptFromInspector; //������������ � ��� ������ �� ���������

    protected List<string> ListOfSpawnUnits = null;
    protected List<string> listOfAdditionalFunctionality = null;

    protected bool _canvasUsed;
    protected bool _inElectricalSystem;

    protected int ChargingPower = 0;

    private void OnEnable() 
    { 
        GlobalUpdate.s_energyTick += EnergyTick;

        CheckingElectricalNetwork();
    }
    private void OnDisable() => GlobalUpdate.s_energyTick -= EnergyTick;

    private void EnergyTick()
    {
        Debug.Log(ChargingPower);
        if (BuildCurrentEnergy > 0 && ChargingPower == 0)
            UsedEnergy(-1);
        else
            UsedEnergy(ChargingPower);
    }


    public void CheckingElectricalNetwork()
    {
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, 5, LayerMask.GetMask("Obstacle")).ToList();
        colliders.Remove(_collider2D);

        if (colliders.Count != 0)
        {
            _buildingNeighborsInterface.Clear();

            List<Collider2D> notAddedInList = new List<Collider2D>();
            List<Collider2D> addedInList = new List<Collider2D>();
            for (int i = 0; i < colliders.Count; i++)
            {
                BuildingInterface colliderInterface = colliders[i].GetComponent<BuildingInterface>();

                MainHeadquarter mainGOScript;
                try
                {
                    mainGOScript = colliderInterface.ReturnTheMainScriptOfTheElectricalNetwork();
                }
                catch(ArgumentException)
                {
                    continue;
                }

                _buildingNeighborsInterface.Add(colliderInterface);

                if (mainGOScript != null)
                {
                    addedInList.Add(colliders[i]);

                    if (_theMainScriptOfTheElectricalNetwork == null)
                    {
                        _inElectricalSystem = true;
                        _theMainScriptOfTheElectricalNetwork = mainGOScript;
                        _theMainScriptOfTheElectricalNetwork.AddInElectricalSystemList(_thisScriptFromInspector);
                        _numberInTheElectricalSystem = _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count - 1;
                    }
                }
                else
                    notAddedInList.Add(colliders[i]);
            }

            for (int i = 0; i < addedInList.Count; i++) 
            {
                GameObject line = Instantiate(_linePrefab, transform.GetChild(0));

                Vector3 dir = addedInList[i].transform.position - transform.position;
                line.transform.position = transform.position + dir / 2;
                line.transform.localScale = new Vector3(dir.magnitude, line.transform.localScale.y, line.transform.localScale.z);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                line.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            StartCoroutine(CheckingTheNeighbors(notAddedInList));
        }
    }

    private IEnumerator CheckingTheNeighbors(List<Collider2D> notAddedInList)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (Collider2D collider in notAddedInList)
            collider.GetComponent<BuildingInterface>().CheckingElectricalNetwork();
    }



    public void Interaction() 
    {
        _canvasUsed = true;
        CanvasControl.Instance.deselectFromCanvas += Deselect;

        CanvasControl.Instance.UsingCanvas(_thisBuildingInfo.Id, $"{BuildCurrentEnergy}/{_thisBuildingInfo.MaxBuildingEnergy}", null, listOfAdditionalFunctionality, _thisScriptFromInspector); 
    }
    public void Deselect()
    {
        _canvasUsed = false;
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
        if (_canvasUsed)
            CanvasControl.Instance.EnergyChangeAction?.Invoke($"{BuildCurrentEnergy}/{_thisBuildingInfo.MaxBuildingEnergy}");
    }

    public List<string> GetListOfSpawnUnits() { return ListOfSpawnUnits; }
    public GameObject GetGameobject() { return this.gameObject; }
    public Collider2D GetBoxCollider() { return _collider2D; }
    public BuildingInterface GetBuildingInterface() { return _thisScriptFromInspector; }
    public BuildingInfo GetBuildingInfo() { return _thisBuildingInfo; }
    public int GetCurrentBuildingEnergy() { return BuildCurrentEnergy; }
    public MainHeadquarter ReturnTheMainScriptOfTheElectricalNetwork() { return _theMainScriptOfTheElectricalNetwork; }
    public WaitingForEnergy ReturnTheWaitingForEnergyScript() { return null; }
    public List<BuildingInterface> GetBuildingNeighbors() { return _buildingNeighborsInterface; }
    public int GetBuildingNumberInElectricalNetwork() { return _numberInTheElectricalSystem; }
}