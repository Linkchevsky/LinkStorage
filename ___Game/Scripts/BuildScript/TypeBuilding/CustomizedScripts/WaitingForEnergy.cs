using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForEnergy : MonoCache, BuildingInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    private BuildingInfo _thisBuildingInfo;
    private BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    private BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();


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
            case "Main Headquarters":
                _requiredUnits = Resources.Load<BuildingInfo>("Builds/MainHeadquarters").NumberOfUnitsToBuild;
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
            case "Main Headquarters":
                gameObject.GetComponent<MainHeadquarters>().enabled = true;
                break;

            case "Electric Pole":
                gameObject.GetComponent<ElectricPole>().enabled = true;
                break;

            case "Generator":
                gameObject.GetComponent<Generator>().enabled = true;
                break;
        }

        _isReady = true;

        Destroy(this.GetComponent<WaitingForEnergy>());
    }


    public override void OnTick() => Allocation();
    private void Allocation()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 1.5f, 0f, LayerMask.GetMask("Unit"));
        Debug.Log(colliders.Length);

        if (colliders.Length != 0)
        {
            foreach (Collider2D collider in colliders)
            {
                UnitInterface unitInterface = collider.GetComponent<UnitInterface>();
                if (_boxCollider.bounds.Contains(unitInterface.GetUnitTarget().position))
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


    public List<string> GetListOfSpawnUnits() { return null; }
    public GameObject GetGameobject() { return this.gameObject; }
    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public BuildingInfo GetBuildingInfo() { return _thisBuildingInfo; }
    public int GetCurrentBuildingEnergy() { throw new ArgumentException("У заготовки не может быть энергии!"); }

    public void UsedEnergy(int amountOfEnergy) => throw new ArgumentException("У заготовки не может быть энергии!");
}
