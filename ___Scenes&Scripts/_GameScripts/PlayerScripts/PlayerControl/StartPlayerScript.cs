using Cinemachine;
using Mirror;
using System.Collections;
using UnityEngine;

public class StartPlayerScript : NetworkBehaviour
{
    private void Start()
    {
        if (!isLocalPlayer) return;

        GameObject.FindWithTag("MainCamera").transform.parent = transform;
    }
}
