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
    [SerializeField] private MainHeadquarter _thisBuildingScriptFromInspector;

    public class ElectricalSystem
    {
        public List<BuildingInterface> ElectricalSystemList = new List<BuildingInterface>();
        public List<BuildingInterface> MainHeadquartersInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> GeneratorsInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> ElectricPolesInElectricalList = new List<BuildingInterface>();

        public int GeneratedEnergy = 0;
    }
    public ElectricalSystem ElectricalSystemInfo = new ElectricalSystem();


    private void Start()
    {
        ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
        listOfAdditionalFunctionality = new List<string> { "spawnUnits" };

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingInfo.MaxBuildingEnergy;

        if (_theMainScriptOfTheElectricalNetwork == null)
        {
            _theMainScriptOfTheElectricalNetwork = _thisBuildingScriptFromInspector;
            AddInElectricalSystemList(_thisScriptFromInspector);
        }
    }

    public void AddInElectricalSystemList(BuildingInterface buildingInterface)
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        switch (buildingInterface.GetBuildingInfo().Id)
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
        for (int i = 0; i < _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count; i++)
            objList.Add(new GameObjectInfo(i));

        for (int i = 0; i < objList.Count; i++)
        {
            foreach (BuildingInterface building in _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[i].GetBuildingNeighbors())
            {
                objList[i].ConnectedObjects.Add(objList[building.GetBuildingNumberInElectricalNetwork()]);
            }
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
                    {
                        Debug.Log(objList[b].id);
                        Debug.Log(objList[b].ConnectedObjects[k].id);
                        objList[b].ConnectedObjects.Remove(objList[k]);
                    }
                }
            }
        }
        List<GameObjectInfo> path = PathFinder.FindPath(objList[startIndex], objList[endIndex]);
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.Log($"{_theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i].id].GetGameobject().name}  +  {path[i].id}");

                List<GameObject> listOfBuildingsGO = new List<GameObject>() { _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i].id].GetGameobject(), _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i + 1].id].GetGameobject() };
                _theMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList[path[i].id].InstallationOfWires(listOfBuildingsGO, true);
            }

            currentPaths.Add(path);

            ChargingPower += 3;
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