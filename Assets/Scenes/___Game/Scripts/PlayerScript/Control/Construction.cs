using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Construction : MonoBehaviour
{
    public static Construction Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }



    private List<GameObject> _buttonsUsed = new List<GameObject>();

    [SerializeField] private PlayerClickControl _playerClickControl;
    [Space]
    [SerializeField] private GameObject _mainHeadquartersButton;
    [SerializeField] private GameObject _testBuildingButton;
    [Space]
    [SerializeField] private GameObject _mainHeadquartersWorkPiece;
    [SerializeField] private GameObject _testBuildingWorkPiece;
    public void ButtonPlacement(List<string> listOfConstructions)
    {
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string UnitName in listOfConstructions)
        {
            switch (UnitName)
            {
                case "MainHeadquarters":
                    _mainHeadquartersButton.SetActive(true);
                    _buttonsUsed.Add(_mainHeadquartersButton);
                    break;

                case "TestBuild":
                    _testBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_testBuildingButton);
                    break;
            }
        }

        for (int i = 0; i < _buttonsUsed.Count; i++)
        {
            _buttonsUsed[i].transform.localPosition = new Vector3(0 - 110 * i, 0, 0);
        }
    }

    private void TurningOffAllButtons()
    {
        _mainHeadquartersButton.SetActive(false);
        _testBuildingButton.SetActive(false);
    }


    public void MainHeadquartersButtonClick() => SpawnWorkPiece(_mainHeadquartersWorkPiece);
    public void TestBuildingButtonClick() => SpawnWorkPiece(_testBuildingWorkPiece);

    private void SpawnWorkPiece(GameObject workPiece)
    {
        _playerClickControl.BuildTransform = Instantiate(workPiece).transform;
        _playerClickControl.CurrentMode = "Construction";
    }

    /*#region[спавн]
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
    #endregion*/
}
