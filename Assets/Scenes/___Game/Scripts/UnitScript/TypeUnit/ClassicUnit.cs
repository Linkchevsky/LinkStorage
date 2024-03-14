using Mirror;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class ClassicUnit : NetworkBehaviour, UnitInterface
{
    private SpecificationsUnit thisUnitSpecifications;

    [SerializeField] private AIDestinationSetter _AIDestinationSetter;
    [SerializeField] private Transform _target;
    [SerializeField] private CircleCollider2D _mainCollider;

    public readonly List<string> ListOfConstructions = new List<string>() { "MainHeadquarters", "TestBuild" };

    private void Start()
    {
        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return;
        }
        UnitControl.Instance.AllUnits.Add(this.gameObject);
        thisUnitSpecifications = SpecificationsUnit.GetUnitData(UnitTypeEnum.ClassicUnit, this.gameObject);
    }

    public void Interaction()
    {
        UnitControl.Instance.AddUnitInSelectedList(this.gameObject);
        CanvasControl.Instance.UsingTheUnitCanvas(GetUnitSpecifications(), ListOfConstructions);

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