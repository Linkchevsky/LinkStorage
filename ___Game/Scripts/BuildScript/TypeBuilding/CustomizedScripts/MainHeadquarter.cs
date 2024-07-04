using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
                TryingToGetPath(ElectricalSystemInfo.ElectricalSystemList.IndexOf(buildingInterface), 0, true);
                break;
        }
    }



    private void TryingToGetPath(int startIndex, int endIndex, bool forEnergy = false)//не все соседи определ€ютс€ быстрее чем происходит выполнение этого кода
    {
        List<Vector3> path;

        try
        {
            path = FindPath(InitializeNodes(), Storage.Instance.AllBuildingsGO[0].transform.position, Storage.Instance.AllBuildingsGO[Storage.Instance.AllBuildingsGO.Count - 1].transform.position);
            path.Insert(0, Storage.Instance.AllBuildingsGO[0].transform.position);
        }
        catch
        {
            TryingToGetPath(startIndex, endIndex, forEnergy);
            return;
        }


        if (path != null && forEnergy)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 wirePosition = (path[i] + path[i + 1]) / 2f;

                if (Storage.Instance.WiresDictionary[wirePosition] != null)
                {
                    Storage.Instance.WiresDictionary[wirePosition].Used(3, "Red");
                }

                continue;
            }
        }
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
                if (!neighbor.Walkable || closedSet.Contains(neighbor) || 
                    Storage.Instance.WiresDictionary[(currentNode.Position + neighbor.Position) / 2].currentEnergy != 0) //отсев по использованию провода
                {
                    continue; // ѕропускаем непроходимые или уже проверенные узлы
                }

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
    private List<Node> GetNeighbors(List<Node> nodes, Node node, float neighborRadius = 5f)
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
}