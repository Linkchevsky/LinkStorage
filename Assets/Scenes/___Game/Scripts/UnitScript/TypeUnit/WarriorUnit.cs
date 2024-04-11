using Mirror;
using Pathfinding;
using UnityEngine;

public class WarriorUnit : NetworkBehaviour, UnitInterface
{
    private SpecificationsUnit thisUnitSpecifications;

    [SerializeField] private AIDestinationSetter _AIDestinationSetter;
    [SerializeField] private BoxCollider2D _mainCollider;

    public Transform _target;

    private void Start()
    {
        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return; 
        }

        UnitControl.Instance.AddUnitInAllUnit(this.gameObject);
        thisUnitSpecifications = SpecificationsUnit.GetUnitData(UnitTypeEnum.WarriorUnit, this.gameObject);
    }

    public void Interaction()
    {
        UnitControl.Instance.AddUnitInSelectedList(this.gameObject);
        CanvasControl.Instance.UsingTheUnitCanvas(GetUnitSpecifications());

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