using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitingForEnergy : MonoCache, BuildingInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    private BuildingInfo _thisBuildingInfo;

    [SerializeField] private WaitingForEnergy _thisScript;
    [SerializeField] private Collider2D _collider2D;
    private BuildingInterface _thisBuildingInterface => this;


    private bool _owned = false;

    [SerializeField] private string _buildType;

    [SyncVar(hook = nameof(IsReadyChange))]
    public bool _isReady = false;

    private void IsReadyChange(bool oldValue, bool newValue) 
    {
        if (!_owned)
            EnableComponent(); 
    }

    public void Started(bool isReady = false)
    {
        _owned = true;
        _isReady = isReady;

        if (isReady)
        {
            EnableComponent();
            return;
        }

        switch (_buildType)
        {
            case "Main Headquarter":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/MainHeadquarter").NumberOfUnitsToBuild;
                break;

            case "Electric Pole":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/ElectricPole").NumberOfUnitsToBuild;
                break;

            case "Generator":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/Generator").NumberOfUnitsToBuild;
                break;
        }
    }

    private void EnableComponent()
    {
        switch (_buildType)
        {
            case "Main Headquarter":
                gameObject.GetComponent<MainHeadquarter>().enabled = true;
                break;

            case "Electric Pole":
                gameObject.GetComponent<ElectricPole>().enabled = true;
                break;

            case "Generator":
                gameObject.GetComponent<Generator>().enabled = true;
                break;
        }

        _isReady = true;

        Destroy(this);
    }

    public void AddInElectricalSystem(List<GameObject> electricalSystemList) { } //��������
    public override void OnTick() => Allocation();
    private void Allocation()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 1.5f, 0f, LayerMask.GetMask("Unit"));

        if (colliders.Length != 0)
        {
            foreach (Collider2D collider in colliders)
            {
                UnitInterface unitInterface = Storage.Instance.AllUnitsInterface[Storage.Instance.AllUnitsColliders.IndexOf(collider)];
                if (_collider2D.bounds.Contains(unitInterface.GetUnitTarget().position))
                {
                    unitInterface.DestroyThisUnit();
                    GetUnit();
                }
            }
        }
    }

    private bool canvasUsed;

    public void Interaction()
    {
        canvasUsed = true;
        CanvasControl.Instance.deselectFromCanvas += Deselect;

        CanvasControl.Instance.UsingCanvas("����� �������������", $"{_currentUnits}/{_requiredUnits}", $"��� ������������� ���: \n{_buildType}");
    }
    public void Deselect()
    {
        canvasUsed = false;
        CanvasControl.Instance.deselectFromCanvas -= Deselect;
    }

    public void GetUnit()
    {
        _currentUnits++;
        CanvasControl.Instance.EnergyChangeAction?.Invoke($"{_currentUnits}/{_requiredUnits}");

        if (_currentUnits >= _requiredUnits)
        {
            if (canvasUsed)
                CanvasControl.Instance.CloseAllCanvasMenu();
            EnableComponent();
        }
    }


    public void InstallationOfWires(List<GameObject> listOfBuildingsGO) => throw new ArgumentException("� ��������� �� ����� ���� ����� � �����!");
    public List<string> GetListOfSpawnUnits() { return null; }
    public GameObject GetGameobject() { return this.gameObject; }
    public Collider2D GetBoxCollider() { return _collider2D; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public BuildingInfo GetBuildingInfo() { return _thisBuildingInfo; }
    public int GetCurrentBuildingEnergy() { throw new ArgumentException("� ��������� �� ����� ���� �������!"); }
    public void UsedEnergy(int amountOfEnergy) => throw new ArgumentException("� ��������� �� ����� ���� �������!");
    public MainHeadquarter ReturnTheMainScriptOfTheElectricalNetwork() => throw new ArgumentException("� ��������� �� ����� ���� �������!");
    public void CheckingElectricalNetwork() { return; }
    public List<BuildingInterface> GetBuildingNeighbors()  { return null; }
    public int GetBuildingNumberInElectricalNetwork() => throw new ArgumentException("� ��������� �� ����� ���� ������!");
}
