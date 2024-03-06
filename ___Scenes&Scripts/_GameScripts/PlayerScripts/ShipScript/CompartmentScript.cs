using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CompartmentScript : NetworkBehaviour
{
    private Tilemap TilemapScript => transform.parent.GetChild(0).GetComponent<Tilemap>();

    public List<GameObject> TileGameObject;
    public List<GameObject> UsedTileGameObject;
}