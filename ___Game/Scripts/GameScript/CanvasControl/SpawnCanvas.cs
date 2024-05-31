using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCanvas : NetworkBehaviour
{
    [SerializeField] private NetworkIdentity _networkIdentity;

    [SerializeField] private GameObject _classicUnitSpawnButton;
    [SerializeField] private GameObject _warriorUnitSpawnButton;

    private List<GameObject> _buttonsUsed = new List<GameObject>();

    private BuildingInterface _buildingInterface;
    private List<string> _listOfSpawnUnits;

    public void ButtonPlacement(BuildingInterface buildingInterface)
    {
        if ((_listOfSpawnUnits = buildingInterface.GetBuildingCharacteristics().ListOfSpawnUnits) == null)
            return;

        _buildingInterface = buildingInterface;
        _listOfSpawnUnits = buildingInterface.GetBuildingCharacteristics().ListOfSpawnUnits;

        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string UnitName in _listOfSpawnUnits)
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
    private void UnitSpawning(string unitId)
    {
        switch (unitId)
        {
            case "ClassicUnit":
                if (_buildingInterface.GetEnergy() - 10 >= 0)
                {
                    _buildingInterface.UsedEnergy(-10);

                    Vector3 spawnPosition = _buildingInterface.GetGameobject().transform.position;
                    CmdClassicUnitSpawnedInNetwork(new Vector3(spawnPosition.x + 1.5f, spawnPosition.y, 0));
                }
                break;

            case "WarriorUnit":
                if (_buildingInterface.GetEnergy() - 20 >= 0)
                {
                    _buildingInterface.UsedEnergy(-20);

                    Vector3 spawnPosition = _buildingInterface.GetGameobject().transform.position;
                    CmdWarriorUnitSpawnedInNetwork(new Vector3(spawnPosition.x + 1.5f, spawnPosition.y, 0));
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

    public void SpawnClassicUnitButton() => UnitSpawning("ClassicUnit");
    public void SpawnWarriorUnitButton() => UnitSpawning("WarriorUnit");
    #endregion
}
