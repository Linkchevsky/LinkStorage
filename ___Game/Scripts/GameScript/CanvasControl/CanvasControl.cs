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

    [HideInInspector] public bool CanvasUsed = false;
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

    //additionalFunctionality - formation, construction, spawnUnits
    public void UsingCanvas(string objectNameText, string energyText = null, string infoText = null, List<string> listOfAdditionalFunctionality = null, 
                            BuildingInterface buildInterface = null, UnitInterface unitInterface = null)
    {
        CanvasUsed = true;

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
    }

    public void CloseAllCanvasMenu()
    {
        CanvasUsed = false;

        _mainPanelGO.SetActive(false);

        _spawnMenuButtonGOList.SetActive(false);
        _formationsButtonGOList.SetActive(false);
        _constructionsButtonGOList.SetActive(false);
    }
    /*
       public void UsingTheBuildCanvas(BuildingInterface buildInterface, GameObject buildGO, List<string> listOfSpawnUnits = null, Transform buildSpawnPoint = null)
       {
           _mainPanelGO.SetActive(true);

           BuildingInfo buildStats = buildInterface.GetBuildingInfo();

           _objectNameText.text = buildStats.Id;
           _infoText.text = $"Energy = {buildInterface.GetCurrentBuildingEnergy()}/{buildStats.MaxBuildingEnergy}";

           if (listOfSpawnUnits != null)
           {
               _spawnMenuButton.SetActive(true);
               _spawnMenuScript.ButtonPlacement(buildInterface, listOfSpawnUnits, buildSpawnPoint);
           }
       }


       public void UsingWaitingForEnergyCanvas(string buildType, int currentUnitsCount, int requiredQuantityUnits, GameObject buildGO = null)
       {
           _mainPanelGO.SetActive(true);

           _objectNameText.text = buildType;
           _infoText.text = $"В процессе постройки\nКол-во юнитов = {currentUnitsCount}/{requiredQuantityUnits}";
       }
       #endregion

       #region[unit]
       public void UsingTheUnitCanvas(UnitInfo unitInfo, UnitInterface unitInterface, GameObject unitGO)
       {
           _mainPanelGO.SetActive(true);

           _objectNameText.text = unitInfo.Id;
           _infoText.text = $"Energy = {unitInterface.GetCurrentUnitEnergy()}/{unitInfo.MaxUnitEnergy}";

           switch(unitInfo.Id)
           {
               case "Classic Unit":
                   _constructionScript.ButtonPlacement();
                   _constructionsButtonGOList.SetActive(true);
                   return;
           }
       }

       public void UsingTheUnitsCanvas(List<GameObject> selectedUnits)
       {
           _mainPanelGO.SetActive(true);

           _objectNameText.text = "Выделена группа юнитов";
           _infoText.text = $"Кол-во юнитов = {selectedUnits.Count}";

           _formationsButtonGOList.SetActive(true);
       }

       #endregion
    */
}
