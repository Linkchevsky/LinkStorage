using Mirror;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BasisOfTheUnits : NetworkBehaviour, UnitInterface
{
    protected UnitInfo _thisUnitInfo;
    [SyncVar] public int UnitCurrentEnergy;

    protected UnitInterface _thisUnitInterface => GetComponent<UnitInterface>();

    [SerializeField] protected AIDestinationSetter _AIDestinationSetter;
    [SerializeField] protected CircleCollider2D _mainCollider;

    [SerializeField] protected Transform _target;


    private void OnEnable() => GlobalUpdate.s_energyTick += EnergyTick;
    private void OnDisable() => GlobalUpdate.s_energyTick -= EnergyTick;
    private void EnergyTick()
    {
        if (UnitCurrentEnergy > 0)
            UsedEnergy(-1);
    }

    public void Interaction()
    {
        UnitControl.Instance.AddUnitInSelectedList(this.gameObject);

        if (_thisUnitInfo.Id == "Classic Unit")
            CanvasControl.Instance.UsingCanvas(_thisUnitInfo.Id, $"{UnitCurrentEnergy}/{_thisUnitInfo.MaxUnitEnergy}", null, new List<string> { "construction" });
        else
            CanvasControl.Instance.UsingCanvas(_thisUnitInfo.Id, $"{UnitCurrentEnergy}/{_thisUnitInfo.MaxUnitEnergy}", null);

        transform.GetChild(0).gameObject.SetActive(true);
        UnitControl.s_cancelingUnitSelection += Deselect;
    }
    private void Deselect()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        UnitControl.s_cancelingUnitSelection -= Deselect;
    }


    public void UnitSetDestination(Vector3 coordinate) => CmdUnitSetDestination(coordinate);

    [Command]
    private void CmdUnitSetDestination(Vector3 coordinate) => RpcUnitSetDestination(coordinate);

    [ClientRpc]
    private void RpcUnitSetDestination(Vector3 coordinate)
    {
        _target.position = new Vector3(coordinate.x, coordinate.y, 0);
        _AIDestinationSetter.target = _target;
    }


    public void DestroyThisUnit() => CmdDestroyThisUnit();

    [Command(requiresAuthority = false)]
    private void CmdDestroyThisUnit() => RpcDestroyThisUnit();

    [ClientRpc]
    private void RpcDestroyThisUnit()
    {
        if (UnitControl.Instance.SelectedUnits.Contains(this.gameObject))
        {
            Deselect();
            UnitControl.Instance.RemoveUnitFromSelectedUnits(this.gameObject);
        }

        UnitControl.Instance.RemoveUnitFromAllUnits(this.gameObject);

        Destroy(this.gameObject);
    }


    public void UsedEnergy(int amountOfEnergy)
    {
        UnitCurrentEnergy += amountOfEnergy;
    }


    public UnitInfo GetUnitInfo()
    {
        return _thisUnitInfo;
    }
    public UnitInterface GetUnitInterface()
    {
        return _thisUnitInterface;
    }
    public int GetCurrentUnitEnergy()
    {
        return UnitCurrentEnergy;
    }
    public Transform GetUnitTarget()
    {
        _AIDestinationSetter.target = null;
        return _target;
    }
}
