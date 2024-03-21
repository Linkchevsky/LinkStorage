using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMenu : NetworkBehaviour
{
    [SerializeField] private NetworkIdentity _networkIdentity;

    [SerializeField] private GameObject _classicUnitSpawnButton;
    [SerializeField] private GameObject _warriorUnitSpawnButton;

    private List<GameObject> _buttonsUsed = new List<GameObject>();

    private Transform _buildingSpawnPoint;

    public void ButtonPlacement(List<string> listOfSpawnUnits, Transform spawnPoint)
    {
        _buildingSpawnPoint = spawnPoint;
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string UnitName in listOfSpawnUnits)
        {
            switch(UnitName)
            {
                case "ClassicUnit":
                    _classicUnitSpawnButton.SetActive(true);
                    _buttonsUsed.Add(_classicUnitSpawnButton);
                    break;

                case "WarriorUnit":
                    _warriorUnitSpawnButton.SetActive(true);
                    _buttonsUsed.Add(_warriorUnitSpawnButton);
                    break;
            }
        }

        for (int i = 0; i < _buttonsUsed.Count; i++)
        {
            _buttonsUsed[i].transform.localPosition = new Vector3(0 - 110 * i, transform.position.y, 0);
        }
    }

    private void TurningOffAllButtons()
    {
        _classicUnitSpawnButton.SetActive(false);
        _warriorUnitSpawnButton.SetActive(false);
    }

    #region[спавн]
    private void UnitSpawning(UnitTypeEnum typeUnit)
    {
        switch (typeUnit)
        {
            case UnitTypeEnum.ClassicUnit:
                CmdClassicUnitSpawnedInNetwork(_buildingSpawnPoint.position);
                break;

            case UnitTypeEnum.WarriorUnit:
                CmdWarriorUnitSpawnedInNetwork(_buildingSpawnPoint.position);
                break;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdClassicUnitSpawnedInNetwork(Vector3 spawnPoint, NetworkConnectionToClient conn = null)
    {
        GameObject _createdUnit = Instantiate(Storage.Instance.ClassicUnitPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, conn);
    }

    [Command(requiresAuthority = false)]
    private void CmdWarriorUnitSpawnedInNetwork(Vector3 spawnPoint, NetworkConnectionToClient conn = null)
    {
        GameObject _createdUnit = Instantiate(Storage.Instance.WarriorUnitPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, conn);
    }

    public void SpawnClassicUnitButton() => UnitSpawning(UnitTypeEnum.ClassicUnit);
    public void SpawnWarriorUnitButton() => UnitSpawning(UnitTypeEnum.WarriorUnit);
    #endregion
}
