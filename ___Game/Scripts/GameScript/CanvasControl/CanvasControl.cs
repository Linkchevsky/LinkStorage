using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    [SerializeField] private GameObject _mainPanelGO;
    [Space]
    [SerializeField] private TextMeshProUGUI _objectNameText;
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _infoText;
    [Space]
    [SerializeField] private SpawnCanvas _spawnMenuScript;
    [SerializeField] private GameObject _spawnMenuButtonGOList;
    [Space]
    [SerializeField] private UnitControl _unitControlScript;
    [SerializeField] private GameObject _formationsButtonGOList;
    [Space]
    [SerializeField] private ConstructionCanvas _constructionScript;
    [SerializeField] private GameObject _constructionsButtonGOList;

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

    public Action<string> EnergyChangeAction;
    public Action<string> InfoChangeChange;

    public Action deselectFromCanvas;

    //additionalFunctionality - formation, construction, spawnUnits
    public void UsingCanvas(string objectNameText, string energyText = null, string infoText = null, List<string> listOfAdditionalFunctionality = null, 
                            BuildingInterface buildInterface = null, UnitInterface unitInterface = null)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = objectNameText;
        _energyText.text = energyText;
        _infoText.text = infoText;

        if (listOfAdditionalFunctionality != null)
            foreach (string additionalFunctionality in listOfAdditionalFunctionality)
            {
                switch (additionalFunctionality)
                {
                    case "formation":
                        _formationsButtonGOList.SetActive(true);
                        break;

                    case "construction":
                        _constructionScript.ButtonPlacement();
                        _constructionsButtonGOList.SetActive(true);
                        break;

                    case "spawnUnits":
                        _spawnMenuScript.ButtonPlacement(buildInterface);
                        _spawnMenuButtonGOList.SetActive(true);
                        break;
                }
            }

        EnergyChangeAction += EneryChange;
        InfoChangeChange += InfoChange;
    }
    private void EneryChange(string newValue) => _energyText.text = newValue;
    private void InfoChange(string newValue) => _energyText.text = newValue;

    public void CloseAllCanvasMenu()
    {
        deselectFromCanvas?.Invoke();

        EnergyChangeAction -= EneryChange;
        InfoChangeChange -= InfoChange;

        _mainPanelGO.SetActive(false);

        _spawnMenuButtonGOList.SetActive(false);
        _formationsButtonGOList.SetActive(false);
        _constructionsButtonGOList.SetActive(false);
    }
}
