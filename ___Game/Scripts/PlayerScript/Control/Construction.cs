using Mirror;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
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

    private string _buildType;

    [HideInInspector] public PlayerClickControl _playerClickControl;

    [SerializeField] private GameObject _mainHeadquartersButton;
    [SerializeField] private GameObject _testBuildingButton;
    [Space]
    [SerializeField] private SpriteRenderer _workPieceSpriteRenderer;

    public readonly List<string> ListOfConstructions = new List<string>() { "MainHeadquarters", "TestBuild" };

    public void ButtonPlacement(string unitType)
    {
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        if (unitType == "ClassicUnit")
        {
            foreach (string buildName in ListOfConstructions)
            {
                switch (buildName)
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


    public void MainHeadquartersButtonClick() => UseWorkPiece("MainHeadquarters");
    public void TestBuildingButtonClick() => UseWorkPiece("TestBuild");

    private void UseWorkPiece(string workPieceType)
    {
        _workPieceSpriteRenderer.gameObject.SetActive(true);

        switch (workPieceType)
        {
            case "MainHeadquarters":
                _buildType = "MainHeadquarters";
                _workPieceSpriteRenderer.transform.localScale = new Vector3(2, 2, 0);
                _workPieceSpriteRenderer.color = Color.green;
                break;
            case "TestBuild":
                _buildType = "TestBuild";
                _workPieceSpriteRenderer.transform.localScale = new Vector3(2, 1, 0);
                _workPieceSpriteRenderer.color = Color.green;
                break;
        }

        _playerClickControl.BuildTransform = _workPieceSpriteRenderer.transform;
        _playerClickControl.CurrentMode = "Construction";
    }

    public void UnitSpawning(Vector3 spawnPoint) 
    {
        if (_workPieceSpriteRenderer.color == Color.green)
        {
            _playerClickControl.CurrentMode = "Movement";

            SpawnedInNetwork(spawnPoint, _buildType);
        }
    }

    [Command(requiresAuthority = false)]
    private void SpawnedInNetwork(Vector3 spawnPoint, string buildType, NetworkConnectionToClient conn = null)
    {
        GameObject _createdBuild = null;
        switch (buildType)
        {
            case "MainHeadquarters":
                _createdBuild = Instantiate(Storage.Instance.MainHeadquartersPrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started(buildType);
                break;

            case "TestBuild":
                _createdBuild = Instantiate(Storage.Instance.TestBuildingPrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started(buildType, true);
                break;
        }
        NetworkServer.Spawn(_createdBuild, conn);

        UpdateMovemedMap();
    }

    [ClientRpc]
    private void UpdateMovemedMap() => AstarPath.active.Scan();

    public void OnCloseClick() => _workPieceSpriteRenderer.gameObject.SetActive(false);
}
