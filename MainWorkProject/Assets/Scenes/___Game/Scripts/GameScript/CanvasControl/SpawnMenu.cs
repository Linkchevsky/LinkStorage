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
    //вся эта срань нужна из-за особенностей Mirror
    [SerializeField] private GameObject _classicUnitPrefabForSpawn;
    [SerializeField] private GameObject _warriorUnitPrefabForSpawn;
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

    [Command]
    private void CmdClassicUnitSpawnedInNetwork(Vector3 spawnPoint)
    {
        GameObject _createdUnit = Instantiate(_classicUnitPrefabForSpawn, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, _networkIdentity.connectionToClient);
    }

    [Command]
    private void CmdWarriorUnitSpawnedInNetwork(Vector3 spawnPoint)
    {
        GameObject _createdUnit = Instantiate(_warriorUnitPrefabForSpawn, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, _networkIdentity.connectionToClient);
    }
    #endregion

    public void SpawnClassicUnitButton() => UnitSpawning(UnitTypeEnum.ClassicUnit);
    public void SpawnWarriorUnitButton() => UnitSpawning(UnitTypeEnum.WarriorUnit);
}
