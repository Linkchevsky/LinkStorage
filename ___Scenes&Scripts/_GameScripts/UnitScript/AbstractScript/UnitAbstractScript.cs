using UnityEngine;
using Mirror;
using Pathfinding;
using UnityEngine.AI;
using Unity.Burst.CompilerServices;
using System.Collections.Generic;
using System;
using TMPro;

public abstract class UnitAbstractScript : NetworkBehaviour
{
    // for move click
    [HideInInspector] public float UnitHp; public float UnitMaxHp = 100;
    [HideInInspector] public float UnitSpeed; public float UnitMaxSpeed = 3;

    private CompartmentScript PreviousCompartmentController;
    private int PrevousCompartmentListIndex;

    [SerializeField] private UnitPathFinder UnitPathFinderScript;
    ////

    private void Start()
    {
        if (!isOwned)
        {
            Destroy(GetComponent<NavMeshAgent>());
            transform.rotation = new Quaternion();
            return;
        }

        UnitSpeed = UnitMaxSpeed;
        UnitHp = UnitMaxHp;

        RaycastHit2D[] Hits = Physics2D.RaycastAll(transform.position, Vector2.zero, 9);
        foreach (RaycastHit2D Hit in Hits)
        {
            if (Hit.transform.gameObject.layer == 9)
            {
                CmdSetParent(this.transform, Hit.transform.parent.parent, Hit.transform.GetComponent<CompartmentScript>());
                return;
            }
        }
    }

    [Command]
    private void CmdSetParent(Transform thisGoTransform, Transform newParent, CompartmentScript CompartmentController)
    {
        thisGoTransform.SetParent(newParent);

        RpcUpdateParent(thisGoTransform, newParent, CompartmentController);
    }

    [ClientRpc]
    private void RpcUpdateParent(Transform thisGoTransform, Transform newParent, CompartmentScript CompartmentController)
    {
        thisGoTransform.SetParent(newParent);
        Walk(CompartmentController);
    }



    public void Walk(CompartmentScript CompartmentController)
    {
        for (int i = 0; i < CompartmentController.TileGameObject.Count; i++)
        {
            if (!CompartmentController.UsedTileGameObject.Contains(CompartmentController.TileGameObject[i]))
            {
                if (PreviousCompartmentController != null)
                    PreviousCompartmentController.UsedTileGameObject.Remove(PreviousCompartmentController.TileGameObject[PrevousCompartmentListIndex]);

                PrevousCompartmentListIndex = i;
                PreviousCompartmentController = CompartmentController;

                UnitPathFinderScript.StartMove(CompartmentController.TileGameObject[i].transform.position);

                CompartmentController.UsedTileGameObject.Add(CompartmentController.TileGameObject[i]);

                GameObject.Find("StartShip").GetComponent<ShipControlScript>().CanMove = true;

                return;
            }
        }
    }
}