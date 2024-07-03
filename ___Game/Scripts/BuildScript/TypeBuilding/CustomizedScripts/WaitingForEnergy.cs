using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasisOfTheBuilding;

public class WaitingForEnergy : MonoCache, BuildingInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    private BuildingInfo _thisBuildingInfo;

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

            case "Battery":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/Battery").NumberOfUnitsToBuild;
                break;
        }
    }

    private void EnableComponent()
    {
        try
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

                case "Battery":
                    gameObject.GetComponent<Battery>().enabled = true;
                    break;
            }
        }
        catch
        {
            EnableComponent();
        }

        _isReady = true;

        Destroy(this);
    }

    public void AddInElectricalSystem(List<GameObject> electricalSystemList) { } //заглушка
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

        CanvasControl.Instance.UsingCanvas("Место строительства", $"{_currentUnits}/{_requiredUnits}", $"Идёт строительство над: \n{_buildType}");
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


    public void InstallationOfWires(List<GameObject> listOfBuildingsGO) => throw new ArgumentException("У заготовки не может быть связи с сетью!");
    public GameObject GetGameobject() { return this.gameObject; }
    public Collider2D GetBoxCollider() { return _collider2D; }
    public void SetBuildingChargingPower(int power) { return; }
    public void UsedEnergy(int amountOfEnergy) => throw new ArgumentException("У заготовки не может быть энергии!");
    public void CheckingElectricalNetwork() { return; }
    public BuildingCharacteristics GetBuildingCharacteristics() { return null; }
    public int GetEnergy() { return 1; }
    public List<ConnectedWire> GetConnectedWiresList() { return null; }
    public void AddedWire(Vector3 coordinatesOfTheTarget, Vector3 wirePosition) { return; }
}
