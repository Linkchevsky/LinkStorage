using Mirror;
using Pathfinding;
using UnityEngine;

public class WarriorUnit : NetworkBehaviour, UnitInterface
{
    private SpecificationsUnit thisUnitSpecifications;

    [SerializeField] private AIDestinationSetter _AIDestinationSetter;
    [SerializeField] private Transform _target;

    private void Start()
    {
        if (!isOwned) return;
        UnitControl.Instance.AllUnits.Add(this.gameObject);
        thisUnitSpecifications = SpecificationsUnit.GetUnitData(UnitTypeEnum.WarriorUnit, this.gameObject);
    }

    public void Interaction()
    {
        UnitControl.Instance.AddUnitInSelectedList(this.gameObject);
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
    public void CmdUnitSetDestination(Vector3 coordinate) => RpcUnitSetDestination(coordinate);

    [ClientRpc]
    public void RpcUnitSetDestination(Vector3 coordinate)
    {
        _target.position = new Vector3(coordinate.x, coordinate.y, 0);
        _AIDestinationSetter.target = _target;
    }



    public SpecificationsUnit GetUnitSpecifications()
    {
        return thisUnitSpecifications;
    }
    public Transform GetUnitTarget()
    {
        _AIDestinationSetter.target = null;
        return _target;
    }
}