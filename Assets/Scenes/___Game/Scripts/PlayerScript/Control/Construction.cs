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
    [SerializeField] private SpriteRenderer _workPieceSpriteRenderer;

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


    public void MainHeadquartersButtonClick() => SpawnWorkPiece("MainHeaquarters");
    public void TestBuildingButtonClick() => SpawnWorkPiece("TestBuilding");

    private void SpawnWorkPiece(string workPieceType)
    {
        _workPieceSpriteRenderer.gameObject.SetActive(true);

        switch (workPieceType)
        {
            case "MainHeaquarters":
                _workPieceSpriteRenderer.transform.localScale = new Vector3(2, 2, 0);
                _workPieceSpriteRenderer.color = Color.green;
                break;
            case "TestBuilding":
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
            _playerClickControl.OnCloseClick();

            SpawnedInNetwork(spawnPoint);
        }
    }

    [Command(requiresAuthority = false)]
    private void SpawnedInNetwork(Vector3 spawnPoint, NetworkConnectionToClient conn = null)
    {
        GameObject _createdUnit = Instantiate(Storage.Instance.MainHeadquartersPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(_createdUnit, conn);
    }

    public void OnCloseClick() => _workPieceSpriteRenderer.gameObject.SetActive(false);
}
