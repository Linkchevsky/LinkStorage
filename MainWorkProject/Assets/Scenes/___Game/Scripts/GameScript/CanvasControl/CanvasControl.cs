using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    [SerializeField] private GameObject _mainPanelGO;
    [Space]
    [SerializeField] private TextMeshProUGUI _objectNameText;
    [SerializeField] private TextMeshProUGUI _infoText;
    [Space]
    [SerializeField] private GameObject _spawnMenuButton;
    [SerializeField] private GameObject _spawnMenuButtonListGO;
    [SerializeField] private SpawnMenu _spawnMenuScript;
    [Space]
    [SerializeField] private GameObject _formationsButtonListGO;
    [SerializeField] private UnitControl _unitControlScript;

    public static CanvasControl Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region[build]
    public void UsingTheBuildCanvas(SpecificationsBuild buildStats, List<string> listOfSpawnUnits = null, Transform buildSpawnPoint = null)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = buildStats.buildType.ToString();
        _infoText.text = $"Energy = {buildStats.buildCurrentEnergy}/{buildStats.buildMaxEnergy}";

        if (listOfSpawnUnits != null)
        {
            _spawnMenuButton.SetActive(true);
            _spawnMenuScript.ButtonPlacement(listOfSpawnUnits, buildSpawnPoint);
        }
    }

    public void UsingTheSpawnMenuButton() => _spawnMenuButtonListGO.SetActive(!_spawnMenuButtonListGO.activeSelf);
    #endregion

    #region[unit]
    public void UsingTheUnitCanvas(SpecificationsUnit unitStats, GameObject selectedUnit)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = unitStats.unitType.ToString();
        _infoText.text = $"Energy = {unitStats.unitCurrentEnergy}/{unitStats.unitMaxEnergy}";
    }

    public void UsingTheUnitsCanvas(List<GameObject> selectedUnits)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = "Выделена группа";
        _infoText.text = $"Кол-во юнитов = {selectedUnits.Count}";

        _formationsButtonListGO.SetActive(true);
    }

    #endregion

    public void CloseAllCanvasMenu()
    {
        _spawnMenuButton.SetActive(false);
        _spawnMenuButtonListGO.SetActive(false);

        _formationsButtonListGO.SetActive(false);

        _mainPanelGO.SetActive(false);
    }
}
