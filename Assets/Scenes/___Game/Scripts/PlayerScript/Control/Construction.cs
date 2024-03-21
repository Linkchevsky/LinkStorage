using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Construction : NetworkBehaviour
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

    [HideInInspector] public PlayerClickControl _playerClickControl;
    [Space]
    [SerializeField] private GameObject _mainHeadquartersButton;
    [SerializeField] private GameObject _testBuildingButton;
    [Space]
    [SerializeField] private NetworkIdentity _networkIdentity;
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


    public void MainHeadquartersButtonClick() => SpawnWorkPiece(Storage.Instance.MainHeadquartersPrefab);
    public void TestBuildingButtonClick() => SpawnWorkPiece(Storage.Instance.TestBuildingPrefab);

    private void SpawnWorkPiece(GameObject workPiece)
    {
        _playerClickControl.BuildTransform = Instantiate(workPiece).transform;
        _playerClickControl.CurrentMode = "Construction";
    }

    public void UnitSpawning(Vector3 spawnPoint) => SpawnedInNetwork(spawnPoint);

    [Command(requiresAuthority = false)]
    private void SpawnedInNetwork(Vector3 spawnPoint, NetworkConnectionToClient conn = null)
    {
        GameObject _createdUnit = Instantiate(Storage.Instance.MainHeadquartersPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, conn);
    }

}
