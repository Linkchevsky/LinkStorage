using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForEnergy : NetworkBehaviour, BuildingInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    private BuildingInfo _thisBuildingInfo;
    private BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    private BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();


    private bool _owned = false;

    [SyncVar]
    private string _buildType;

    [SyncVar(hook = nameof(IsReadyChange))]
    public bool _isReady = false;

    private void IsReadyChange(bool oldValue, bool newValue) 
    {
        if (!_owned)
            EnableComponent(); 
    }

    public void Started(string buildType, bool isReady = false)
    {
        _owned = true;

        _buildType = buildType;
        _isReady = isReady;

        if (isReady)
        {
            EnableComponent();
            return;
        }

        switch (_buildType)
        {
            case "Main Headquarters":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/MainHeadquarters").NumberOfUnitsToBuild;
                break;

            case "Electric Pole":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/ElectricPole").NumberOfUnitsToBuild;
                break;
        }
    }

    public void GetUnit()
    {
        _currentUnits++;
        CanvasControl.Instance.EnergyChangeAction?.Invoke($"{_currentUnits}/{_requiredUnits}");

        if (_currentUnits >= _requiredUnits)
            EnableComponent();
    }

    private void EnableComponent()
    {
        switch (_buildType)
        {
            case "Main Headquarters":
                if (CanvasControl.Instance.CanvasUsed)
                    CanvasControl.Instance.CloseAllCanvasMenu();

                gameObject.GetComponent<MainHeadquarters>().enabled = true;
                break;

            case "Electric Pole":
                if (CanvasControl.Instance.CanvasUsed)
                    CanvasControl.Instance.CloseAllCanvasMenu();

                gameObject.GetComponent<MainHeadquarters>().enabled = true;
                break;
        }


        _isReady = true;

        Destroy(this.GetComponent<WaitingForEnergy>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Unit"))
        {
            UnitInterface unitInterface = collision.GetComponent<UnitInterface>();
            if (_boxCollider.bounds.Contains(unitInterface.GetUnitTarget().position))
            {
                unitInterface.DestroyThisUnit();
                GetUnit();
            }
        }
    }

    public void Interaction() => CanvasControl.Instance.UsingCanvas("Место строительства", $"{_currentUnits}/{_requiredUnits}", $"Идёт строительство над: \n{_buildType}");


    public List<string> GetListOfSpawnUnits() { return null; }
    public GameObject GetGameobject() { return this.gameObject; }
    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public BuildingInfo GetBuildingInfo() { return _thisBuildingInfo; }
    public int GetCurrentBuildingEnergy() { throw new ArgumentException("У заготовки не может быть энергии!"); }

    public void UsedEnergy(int amountOfEnergy) => throw new ArgumentException("У заготовки не может быть энергии!");
}
