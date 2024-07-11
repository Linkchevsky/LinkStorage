using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using Debug = UnityEngine.Debug;

public class MainHeadquarter : BasisOfTheBuilding
{
    [Space]
    [SerializeField] private MainHeadquarter _thisMainHeadquarterScriptFromInspector;

    public int ChargingPower = 0;

    public class ElectricalSystem
    {
        public List<BuildingInterface> ElectricalSystemList = new List<BuildingInterface>();
        public List<MainHeadquarter> MainHeadquartersInElectricalList = new List<MainHeadquarter>();
        public List<Generator> GeneratorsInElectricalList = new List<Generator>();
        public List<ElectricPole> ElectricPolesInElectricalList = new List<ElectricPole>();

        public int AllGeneratedEnergy = 0;
        public int FreeChargingPower = 0;
    }
    public ElectricalSystem ElectricalSystemInfo = new ElectricalSystem();



    private void Start()
    {
        BuildingCharacteristics.ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
        BuildingCharacteristics.listOfAdditionalFunctionality = new List<string> { "spawnUnits" };

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        if (BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
            BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork = _thisMainHeadquarterScriptFromInspector;

        AddMainHeadquarterInElectricalSystemList(BuildingCharacteristics.ThisScriptFromInspector, this);
        BuildingCharacteristics.NumberInTheElectricalSystem = ElectricalSystemInfo.ElectricalSystemList.Count - 1;

        CheckingElectricalNetwork();
    }



    public void AddMainHeadquarterInElectricalSystemList(BuildingInterface buildingInterface, MainHeadquarter mainHeadquarterClass) 
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        ElectricalSystemInfo.MainHeadquartersInElectricalList.Add(mainHeadquarterClass); 
    }



    public void AddElectricPoleInElectricalSystemList(BuildingInterface buildingInterface, ElectricPole electricPoleClass)
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        ElectricalSystemInfo.ElectricPolesInElectricalList.Add(electricPoleClass);
    }



    public void AddGeneratorInElectricalSystemList(BuildingInterface buildingInterface, Generator generatorClass)
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        ElectricalSystemInfo.AllGeneratedEnergy += 3;
        ElectricalSystemInfo.FreeChargingPower += 3;
        ElectricalSystemInfo.GeneratorsInElectricalList.Add(generatorClass);
    }



    //передача энергии от любого свободного генератора (startIndex ,будет использоватьс€ дл€ выбора кокретного генератора)
    public bool EnergyTransfer(int startIndex, int endIndex, int energyCount)
    {
        List<Generator> usedGeneratorsScriptList = new List<Generator>();
        int unallocatedEnergy = energyCount;

        foreach (Generator generatorScript in ElectricalSystemInfo.GeneratorsInElectricalList)
        {
            if (generatorScript.EnergyGenerated - generatorScript.EnergyOutput != 0)
            {
                usedGeneratorsScriptList.Add(generatorScript);
                unallocatedEnergy -= generatorScript.EnergyGenerated - generatorScript.EnergyOutput;
                if (unallocatedEnergy <= 0)
                    break;
            }
        }

        if (unallocatedEnergy > 0)
            return false;

        foreach (Generator generatorScript in usedGeneratorsScriptList)
        {
            if (!TryingToGetPath(generatorScript.BuildingCharacteristics.NumberInTheElectricalSystem, endIndex, energyCount, generatorScript))
                return false;
        }

        return true;
    }



    #region[поиск пути]
    private int _theAmountOfEnergyTransmittedAlongThePath = 0; //количество передаваемой по пути энергии
    public bool TryingToGetPath(int startIndex, int endIndex, int energyCount, Generator usedGeneratorScript)
    {
        List<Vector3> path;
        _theAmountOfEnergyTransmittedAlongThePath = energyCount;

        path = FindPath(InitializeNodes(), _thisMainHeadquarterScriptFromInspector.transform.position, ElectricalSystemInfo.ElectricalSystemList[startIndex].GetGameobject().transform.position);

        if (path == null)
            return false;

        path.Insert(0, Storage.Instance.AllBuildingsGO[0].transform.position);

        usedGeneratorScript.EnergyOutput += _theAmountOfEnergyTransmittedAlongThePath;

        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 wirePosition = (path[i] + path[i + 1]) / 2f;
                wirePosition = new Vector3((float)Math.Round(wirePosition.x * 100) / 100, (float)Math.Round(wirePosition.y * 100) / 100, 0);

                if (Storage.Instance.WiresDictionary[wirePosition] != null)
                {
                    Storage.Instance.WiresDictionary[wirePosition].Used(_theAmountOfEnergyTransmittedAlongThePath, "Red");
                }

                continue;
            }
        }

        if (energyCount - _theAmountOfEnergyTransmittedAlongThePath != 0)
            TryingToGetPath(startIndex, endIndex, energyCount - _theAmountOfEnergyTransmittedAlongThePath, usedGeneratorScript);

        return true;
    }

    public List<Vector3> FindPath(List<Node> nodes, Vector3 start, Vector3 target)
    {
        Node startNode = GetClosestNode(nodes, start); // ѕолучаем узел, ближайший к стартовой позиции
        Node targetNode = GetClosestNode(nodes, target); // ѕолучаем узел, ближайший к целевой позиции

        List<Node> openSet = new List<Node>(); // ќткрытый список узлов дл€ проверки
        HashSet<Node> closedSet = new HashSet<Node>(); // «акрытый список узлов, которые уже проверены

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i]; // Ќаходим узел с наименьшей стоимостью F
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode); // ≈сли достигли целевого узла, строим путь
            }

            foreach (Node neighbor in GetNeighbors(nodes, currentNode))
            {
                Vector3 wirePosition = (currentNode.Position + neighbor.Position) / 2;
                wirePosition = new Vector3((float)Math.Round(wirePosition.x * 100) / 100, (float)Math.Round(wirePosition.y * 100) / 100, 0);

                int freeEnergy = Storage.Instance.WiresDictionary[wirePosition].maxEnergy - Storage.Instance.WiresDictionary[wirePosition].currentEnergy;

                if (!neighbor.Walkable || closedSet.Contains(neighbor) || freeEnergy == 0) //ѕропускаем по кол-ву свободной энергии в проводе и непроходимые или уже проверенные узлы
                    continue;

                if (_theAmountOfEnergyTransmittedAlongThePath > freeEnergy)
                    _theAmountOfEnergyTransmittedAlongThePath = freeEnergy;

                float newMovementCostToNeighbor = currentNode.GCost + Vector3.Distance(currentNode.Position, neighbor.Position);
                if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = Vector3.Distance(neighbor.Position, targetNode.Position);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor); // ƒобавл€ем узел в открытый список дл€ дальнейшей проверки
                    }
                }
            }
        }

        return null; // ¬озвращаем null, если путь не найден
    }

    // ћетод дл€ получени€ ближайшего узла к заданной позиции
    private Node GetClosestNode(List<Node> nodes, Vector3 position)
    {
        Node closestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(position, node.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    // ћетод дл€ получени€ соседних узлов
    private List<Node> GetNeighbors(List<Node> nodes, Node node, float neighborRadius = 5.2f)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var potentialNeighbor in nodes)
            if (potentialNeighbor != node && Vector3.Distance(node.Position, potentialNeighbor.Position) <= neighborRadius)
                neighbors.Add(potentialNeighbor); // ƒобавл€ем узел в список соседей, если он находитс€ в пределах радиуса

        return neighbors;
    }

    // ћетод дл€ построени€ пути, возвращает список позиций
    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent; // ѕереходим к родительскому узлу
        }

        path.Reverse(); // –азворачиваем путь, чтобы получить его от старта до цели
        return path;
    }
    List<Node> InitializeNodes()
    {
        List<Node> nodes = new List<Node>();

        // «аполн€ем список узлов на основе списка позиций
        foreach (GameObject GO in Storage.Instance.AllBuildingsGO)
            nodes.Add(new Node(GO.transform.position, true)); // «наем что все узлы проходимы

        return nodes;
    }
}

public class Node
{
    public Vector3 Position; // ѕозици€ узла в пространстве
    public bool Walkable; // ћожно ли пройти через этот узел
    public float GCost; // —тоимость от старта до этого узла
    public float HCost; // ѕредполагаема€ стоимость от этого узла до цели
    public float FCost => GCost + HCost; // ќбща€ стоимость (G + H)
    public Node Parent; // –одительский узел

    public Node(Vector3 position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
    }
    #endregion
}