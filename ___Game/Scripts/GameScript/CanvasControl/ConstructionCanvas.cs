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

    [SerializeField] private GameObject _mainHeadquarterBuildingButton;
    [SerializeField] private GameObject _electricPoleBuildingButton;
    [SerializeField] private GameObject _generatorBuildingButton;
    [SerializeField] private GameObject _batteryButton;
    [Space]
    [SerializeField] private WorkPiece _workPieceScript;

    public readonly List<string> ListOfConstructions = new List<string>() { "Main Headquarter", "Electric Pole", "Generator", "Battery" };

    public void ButtonPlacement()
    {
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string buildName in ListOfConstructions)
        {
            switch (buildName)
            {
                case "Main Headquarter":
                    _mainHeadquarterBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_mainHeadquarterBuildingButton);
                    break;

                case "Electric Pole":
                    _electricPoleBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_electricPoleBuildingButton);
                    break;

                case "Generator":
                    _generatorBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_generatorBuildingButton);
                    break;

                case "Battery":
                    _batteryButton.SetActive(true);
                    _buttonsUsed.Add(_batteryButton);
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
        _mainHeadquarterBuildingButton.SetActive(false);
        _electricPoleBuildingButton.SetActive(false);
        _generatorBuildingButton.SetActive(false);
        _batteryButton.SetActive(false);
    }


    public void MainHeadquartersButtonClick() => UseWorkPiece("Main Headquarter");
    public void ElectricPoleButtonClick() => UseWorkPiece("Electric Pole");
    public void GeneratorButtonClick() => UseWorkPiece("Generator");
    public void BatteryButtonClick() => UseWorkPiece("Battery");

    private void UseWorkPiece(string workPieceType)
    {
        _workPieceScript.gameObject.SetActive(true);

        _workPieceScript.Used(workPieceType);
        _buildType = workPieceType;

        _playerClickControl.BuildTransform = _workPieceScript.transform;
        _playerClickControl.CurrentMode = "Construction";
    }

    public void BuildSpawning(Vector3 spawnPoint) 
    {
        if (_workPieceScript.SpriteRendererReturn().color == Color.green)
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
            case "Main Headquarter":
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

            case "Battery":
                _createdBuild = Instantiate(Storage.Instance.BatteryPrefab, spawnPoint, Quaternion.identity);
                _createdBuild.GetComponent<WaitingForEnergy>().Started();
                break;
        }
        NetworkServer.Spawn(_createdBuild, conn);

        UpdateMovemedMap();
    }

    [ClientRpc]
    private void UpdateMovemedMap() => AstarPath.active.Scan();

    public void OnCloseClick() => _workPieceScript.gameObject.SetActive(false);
}
