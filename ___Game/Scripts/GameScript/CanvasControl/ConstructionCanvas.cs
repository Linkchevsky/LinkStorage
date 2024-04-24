using Mirror;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionCanvas : NetworkBehaviour
{
    public static ConstructionCanvas Instance;
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

    [SerializeField] private GameObject _mainHeadquartersBuildingButton;
    [SerializeField] private GameObject _electricPoleBuildingButton;
    [SerializeField] private GameObject _generatorBuildingButton;
    [Space]
    [SerializeField] private SpriteRenderer _workPieceSpriteRenderer;

    public readonly List<string> ListOfConstructions = new List<string>() { "Main Headquarters", "Electric Pole", "Generator" };

    public void ButtonPlacement()
    {
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string buildName in ListOfConstructions)
        {
            switch (buildName)
            {
                case "Main Headquarters":
                    _mainHeadquartersBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_mainHeadquartersBuildingButton);
                    break;

                case "Electric Pole":
                    _electricPoleBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_electricPoleBuildingButton);
                    break;

                case "Generator":
                    _generatorBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_generatorBuildingButton);
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
        _mainHeadquartersBuildingButton.SetActive(false);
        _electricPoleBuildingButton.SetActive(false);
        _generatorBuildingButton.SetActive(false);
    }


    public void MainHeadquartersButtonClick() => UseWorkPiece("Main Headquarters");
    public void ElectricPoleButtonClick() => UseWorkPiece("Electric Pole");
    public void GeneratorButtonClick() => UseWorkPiece("Generator");

    private void UseWorkPiece(string workPieceType)
    {
        _workPieceSpriteRenderer.gameObject.SetActive(true);

        switch (workPieceType)
        {
            case "Main Headquarters":
                _buildType = "Main Headquarters";
                _workPieceSpriteRenderer.transform.localScale = new Vector3(2, 2, 0);
                break;
            case "Electric Pole":
                _buildType = "Electric Pole";
                _workPieceSpriteRenderer.transform.localScale = new Vector3(1, 1, 0);
                break;
            case "Generator":
                _buildType = "Generator";
                _workPieceSpriteRenderer.transform.localScale = new Vector3(2, 1, 0);
                break;
        }

        _workPieceSpriteRenderer.color = Color.green;

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
            case "Main Headquarters":
                _createdBuild = Instantiate(Storage.Instance.MainHeadquartersPrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started();
                break;

            case "Electric Pole":
                _createdBuild = Instantiate(Storage.Instance.ElectricPolePrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started();
                break;

            case "Generator":
                _createdBuild = Instantiate(Storage.Instance.GeneratorPrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started();
                break;
        }
        NetworkServer.Spawn(_createdBuild, conn);

        UpdateMovemedMap();
    }

    [ClientRpc]
    private void UpdateMovemedMap() => AstarPath.active.Scan();

    public void OnCloseClick() => _workPieceSpriteRenderer.gameObject.SetActive(false);
}
