using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MainHeadquarter : BasisOfTheBuilding
{
    [Space]
    [SerializeField] private MainHeadquarter _thisMainHeadquarterScriptFromInspector;

    public int ChargingPower = 0;
    public int FreeChargingPower = 0;

    public class ElectricalSystem
    {
        public List<BuildingInterface> ElectricalSystemList = new List<BuildingInterface>();
        public List<BuildingInterface> MainHeadquartersInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> GeneratorsInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> ElectricPolesInElectricalList = new List<BuildingInterface>();

        public List<Wire> AllWiresList = new List<Wire>();

        public int GeneratedEnergy = 0;
    }
    public ElectricalSystem ElectricalSystemInfo = new ElectricalSystem();


    public void SetChargingPower(int power)
    {
        ChargingPower += power;
        FreeChargingPower += power;

        SetBuildingChargingPower(1);
    }

    private void Start()
    {
        _buildingCharacteristics.ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
        _buildingCharacteristics.listOfAdditionalFunctionality = new List<string> { "spawnUnits" };

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        if (_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
        {
            _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork = _thisMainHeadquarterScriptFromInspector;
            AddInElectricalSystemList(_buildingCharacteristics.ThisScriptFromInspector);
        }
    }

    public void AddInElectricalSystemList(BuildingInterface buildingInterface)
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        switch (buildingInterface.GetBuildingCharacteristics().ThisBuildingInfo.Id)
        {
            case "Main Headquarter":
                ElectricalSystemInfo.MainHeadquartersInElectricalList.Add(buildingInterface);
                break;

            case "Electric Pole":
                ElectricalSystemInfo.ElectricPolesInElectricalList.Add(buildingInterface);
                break;

            case "Generator":
                ElectricalSystemInfo.GeneratedEnergy += 3;
                ElectricalSystemInfo.GeneratorsInElectricalList.Add(buildingInterface);
                StartCoroutine(Timer(ElectricalSystemInfo.ElectricalSystemList.IndexOf(buildingInterface), 0));
                break;
        }
    }



    private IEnumerator Timer(int startIndex, int endIndex)//не все соседи определяются быстрее чем происходит выполнение этого кода
    {
        yield return new WaitForSeconds(0.1f);

        Path(startIndex, endIndex);
    }

    public List<List<GameObjectInfo>> currentPaths = new List<List<GameObjectInfo>>();

    public void Path(int startIndex, int endIndex)
    {
        List<GameObjectInfo> objList = new List<GameObjectInfo>();
        for (int i = 0; i < _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count; i++)
            objList.Add(new GameObjectInfo(i));

        for (int i = 0; i < objList.Count; i++)
        {
            foreach (BuildingInterface building in _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[i].GetBuildingCharacteristics().BuildingNeighborsInterface)
                objList[i].ConnectedObjects.Add(objList[building.GetBuildingCharacteristics().NumberInTheElectricalSystem]);
        }

        if (currentPaths.Count > 0)
        {
            foreach (List<GameObjectInfo> usedPath in currentPaths)
            {
                for (int i = 0; i < usedPath.Count - 1; i++)
                {
                    int b, k;
                    b = usedPath[i].id;
                    k = usedPath[i + 1].id;

                    if (b < objList.Count && k < objList[b].ConnectedObjects.Count)
                        objList[b].ConnectedObjects.Remove(objList[k]);
                }
            }
        }
        List<GameObjectInfo> path = PathFinder.FindPath(objList[startIndex], objList[endIndex]);
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                int[] listOfBuildingsNumbers = new int[2] { _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i].id].GetBuildingCharacteristics().NumberInTheElectricalSystem,
                    _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i + 1].id].GetBuildingCharacteristics().NumberInTheElectricalSystem }; //получение номеров закрашиваемых проводов

                foreach(Wire wireScript in ElectricalSystemInfo.AllWiresList)
                {
                    if (wireScript.buildingsNumbers[0] == listOfBuildingsNumbers[0] && wireScript.buildingsNumbers[1] == listOfBuildingsNumbers[1])
                    {
                        wireScript.Used(3);
                        break;
                    }
                }
            }

            currentPaths.Add(path);

            SetChargingPower(3);
        }
        else
            Debug.Log("Путь не найден!");
    }

    public class GameObjectInfo
    {
        public int id;
        public List<GameObjectInfo> ConnectedObjects;

        public GameObjectInfo(int id)
        {
            this.id = id;
            ConnectedObjects = new List<GameObjectInfo>();
        }
    }

    public class PathFinder
    {
        public static List<GameObjectInfo> FindPath(GameObjectInfo start, GameObjectInfo target)
        {
            Queue<List<GameObjectInfo>> queue = new Queue<List<GameObjectInfo>>();
            HashSet<GameObjectInfo> visited = new HashSet<GameObjectInfo>();

            queue.Enqueue(new List<GameObjectInfo> { start });

            while (queue.Count > 0)
            {
                List<GameObjectInfo> path = queue.Dequeue();
                GameObjectInfo currentNode = path[path.Count - 1];

                if (currentNode == target)
                {
                    return path;
                }

                if (!visited.Contains(currentNode))
                {
                    visited.Add(currentNode);

                    foreach (GameObjectInfo connectedNode in currentNode.ConnectedObjects)
                    {
                        List<GameObjectInfo> newPath = new List<GameObjectInfo>(path);
                        newPath.Add(connectedNode);
                        queue.Enqueue(newPath);
                    }
                }
            }

            return null;
        }
    }
}