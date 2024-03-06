using Cinemachine;
using Mirror;
using Pathfinding;
using System.Collections;
using UnityEngine;

public class StartPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject _theMainHeadquarters;
    [SerializeField] private NetworkIdentity _networkIdentity;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(transform.GetChild(0).gameObject);
            return; 
        }

        transform.GetChild(0).gameObject.SetActive(true);

        GameObject CameraGO = GameObject.FindWithTag("MainCamera");

        CameraGO.transform.parent = transform;
        CameraGO.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        CameraGO.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;

        CmdStartSpawnBuilds();
    }

    [Command]
    private void CmdStartSpawnBuilds()
    {
        GameObject TheCreatedGO = Instantiate(_theMainHeadquarters, transform.position, Quaternion.identity);
        NetworkServer.Spawn(TheCreatedGO, _networkIdentity.connectionToClient);

        StartSpawnBuilds();
    }

    private void StartSpawnBuilds() => AstarPath.active.Scan();
}
