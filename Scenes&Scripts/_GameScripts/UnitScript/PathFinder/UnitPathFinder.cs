using Mirror;
using NUnit.Framework;
using Pathfinding;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitPathFinder : NetworkBehaviour
{
    private List<Vector3> UnitPoint = new List<Vector3>();
    [SerializeField] private Seeker SeekerScript;

    [SerializeField] private Animator UnitAnimator;

    private Path path;
    private int currentWaypoint = 0;

    public bool CanMove;

    public void StartMove(Vector3 targetPosition)
    {
        CanMove = false;
        AstarPath.active.Scan();
        SeekerScript.StartPath(this.transform.position, targetPosition, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error && isOwned)
        {
            path = p;
            currentWaypoint = 0;
        }
        else return;

        if (UnitPoint.Count > 0)
            UnitPoint.Clear();

        for (int i = 1; i < path.vectorPath.Count; i++)
        {
            Vector3 LocalCoordinate = transform.parent.InverseTransformPoint(path.vectorPath[i]);
            if (IfRoundedUp(LocalCoordinate.x, LocalCoordinate.y))
            {
                Vector3 CalculatedVector = new Vector3(RoundingANumber(LocalCoordinate.x), RoundingANumber(LocalCoordinate.y), -1);
                if (!UnitPoint.Contains(CalculatedVector) && CalculatedVector != transform.localPosition)
                    UnitPoint.Add(CalculatedVector);
            }
        }
        
        if (UnitPoint.Count > 0)
            CmdChangeList(UnitPoint);
    }

    private readonly SyncList<Vector3> SyncUnitPoint = new SyncList<Vector3>();

    [Command]
    private void CmdChangeList(List<Vector3> CmdUnitPoint)
    {
        SyncUnitPoint.Clear();
        SyncUnitPoint.AddRange(CmdUnitPoint);

        RpcChangeList();
    }

    [ClientRpc]
    private void RpcChangeList()
    {
        currentWaypoint = 0;
        CanMove = true;
    }

    private bool IfRoundedUp(float NumberX, float NumberY)
    {
        if (Math.Abs(NumberX) % 1 >= 0.25 && Math.Abs(NumberX) % 1 < 0.75 && Math.Abs(NumberY) % 1 >= 0.25 && Math.Abs(NumberY) % 1 < 0.75)
        {
            return true;
        }
        return false;
    }

    private float RoundingANumber(float Number)
    {
        if (Number < 0)
            Number = (int)Number - 0.5f;
        else
            Number = (int)Number + 0.5f;
        return Number;
    }

    private void Update()
    {
        if (!CanMove)
            return;

        UnitAnimator.SetBool("IsWalked", true);

        Vector3 direction = (SyncUnitPoint[currentWaypoint] - transform.localPosition).normalized;
        direction = new Vector3(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), direction.z);

        transform.localPosition += direction * 2 * Time.deltaTime;

        transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90, Vector3.forward);

        if (Vector3.Distance(transform.localPosition, SyncUnitPoint[currentWaypoint]) < 0.01)
        {
            currentWaypoint++;

            if (currentWaypoint >= SyncUnitPoint.Count)
            {
                CanMove = false;
                UnitAnimator.SetBool("IsWalked", false);
                transform.localPosition = new Vector3(RoundingANumber(transform.localPosition.x), RoundingANumber(transform.localPosition.y), -1);
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                return;
            }
        }
    }
}
