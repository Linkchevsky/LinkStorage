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
    private BuildingInterface _buildingInterface;
    private List<string> _listOfSpawnUnits;

    public void ButtonPlacement(BuildingInterface buildingInterface, List<string> listOfSpawnUnits, Transform spawnPoint)
    {
        _buildingSpawnPoint = spawnPoint;
        _buildingInterface = buildingInterface;
        _listOfSpawnUnits = listOfSpawnUnits;


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
        SpecificationsBuilding buildingStats = _buildingInterface.GetBuildingStats();

        switch (typeUnit)
        {
            case UnitTypeEnum.ClassicUnit:
                if (_buildingInterface.GetCurrentBuildingEnergy() - 10 >= 0)
                {
                    _buildingInterface.RemoveEnergy(10);
                    CanvasControl.Instance.UsingTheBuildCanvas(_buildingInterface, _listOfSpawnUnits, _buildingSpawnPoint);

                    CmdClassicUnitSpawnedInNetwork(_buildingSpawnPoint.position);
                }
                break;

            case UnitTypeEnum.WarriorUnit:
                if (_buildingInterface.GetCurrentBuildingEnergy() - 20 >= 0)
                {
                    _buildingInterface.RemoveEnergy(20);
                    CanvasControl.Instance.UsingTheBuildCanvas(_buildingInterface, _listOfSpawnUnits, _buildingSpawnPoint);

                    CmdWarriorUnitSpawnedInNetwork(_buildingSpawnPoint.position);
                }
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
