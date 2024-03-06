using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class PlayerSpawnUnitScript : NetworkBehaviour
{
    [SerializeField] private GameObject UnitGO;

    [SerializeField] private NetworkIdentity PlayerNetworkIdentity;

    private Vector3 SpawnPosition; //стартовая позиция

    private UnitManagerScript PlayerUnitManager;

    private void Start()
    {
        if (!isLocalPlayer) { Destroy(transform.GetChild(0).gameObject); return; }

        PlayerUnitManager = UnitManagerScript.Instance;

        SpawnPosition = transform.position;
    }

    public void SpawnUnitButtonClick()
    {
        if (isLocalPlayer)
            CmdSpawnUnitButtonClick(PlayerNetworkIdentity, PlayerUnitManager, SpawnPosition);
    }

    [Command]
    private void CmdSpawnUnitButtonClick(NetworkIdentity CmdPlayerNetworkIdentity, UnitManagerScript CmdPlayerUnitManager, Vector3 CmdSpawnPosition)
    {
        GameObject TheCreatedUnit = Instantiate(UnitGO, new Vector3(CmdSpawnPosition.x, CmdSpawnPosition.y, -1), Quaternion.identity);
        NetworkServer.Spawn(TheCreatedUnit);
        TheCreatedUnit.GetComponent<NetworkIdentity>().AssignClientAuthority(CmdPlayerNetworkIdentity.connectionToClient); //присваивает права на объект тому, кто спавнит. Это даёт: синхронизацию положения объекта для остальных игроков, удаления при выходе владельца, управление только владельцем

        RpcAddInAllUnitList(TheCreatedUnit, CmdPlayerUnitManager);
        TargetAddInList(CmdPlayerNetworkIdentity.connectionToClient, TheCreatedUnit);
    }

    [ClientRpc]
    private void RpcAddInAllUnitList(GameObject CmdUnit, UnitManagerScript CmdPlayerUnitManager)
    {
        CmdPlayerUnitManager.AllUnit.Add(CmdUnit);
    }

    [TargetRpc]
    private void TargetAddInList(NetworkConnectionToClient target, GameObject CmdUnit)
    {
        PlayerUnitManager.PlayerUnit.Add(CmdUnit);
    }
}
