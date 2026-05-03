using Lucidity;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Diagnostics;
using System;
using UnityEditor;


public enum AlgorithmType
{
    AStar,
    Dijkstra,
    GreedyBFS,
    BFS
}

public class PathFinding : MonoBehaviour
{
    public AlgorithmType currentAlgorithm = AlgorithmType.AStar;
    PathRequestManager requestManager;
    Grid3D grid3D;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();

        grid3D = GetComponent<Grid3D>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        int nodesExplored = 0;

        Node startNode = grid3D.NodeFromWorldPoint(startPos);
        Node targetNode = grid3D.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            // Limpio la grid porque cada algoritmo calcula los costos distintos
            grid3D.ResetGrid();

            switch (currentAlgorithm)
            {
                case AlgorithmType.AStar:
                    pathSuccess = AStar(startNode, targetNode, ref nodesExplored);
                    break;
                case AlgorithmType.Dijkstra:
                    pathSuccess = Dijkstra(startNode, targetNode, ref nodesExplored);
                    break;
                case AlgorithmType.GreedyBFS:
                    pathSuccess = Greedy(startNode, targetNode, ref nodesExplored);
                    break;
                case AlgorithmType.BFS:
                    pathSuccess = BFS(startNode, targetNode, ref nodesExplored);
                    break;
            }
        }

        sw.Stop();


        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            UnityEngine.Debug.Log($"Algoritmo: {currentAlgorithm} | Tiempo: {sw.ElapsedMilliseconds} ms | Nodos Explorados: {nodesExplored}");
        }
        else
        {
            UnityEngine.Debug.Log($"Algoritmo: {currentAlgorithm} | No se encontró camino | Nodos Explorados: {nodesExplored}");
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        yield return null;
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Invierto el path para ponerlo de principio a fin
        path.Reverse();
        Vector3[] waypoints = SimplifyPath(path);

        // SimplifyPath devuelve array asi que puedes entrar por donde quieras
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count; i++)
        {

            waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        // la Y es la z
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }


    // Coste = gCost (distancia desde inicio) + hCost (distancia al final)
    public bool AStar(Node startNode, Node targetNode, ref int nodesExplored)
    {
        Heap<Node> openSet = new Heap<Node>(grid3D.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0; // La distancia al principio es 0
        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            nodesExplored++;
            closedSet.Add(currentNode);

            if (currentNode == targetNode) return true;

            foreach (Node neighbour in grid3D.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    else openSet.UpdateItem(neighbour);
                }
            }
        }
        return false;
    }

    public bool BFS(Node startNode, Node targetNode, ref int nodesExplored)
    {
        Queue<Node> openSet = new Queue<Node>(grid3D.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Enqueue(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();
            nodesExplored++;
            closedSet.Add(currentNode);

            if (currentNode == targetNode) return true;

            foreach (Node neighbour in grid3D.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                if (!openSet.Contains(neighbour))
                {
                    neighbour.parent = currentNode;
                    openSet.Enqueue(neighbour);
                }
                ;
            }
        }
        return false;
    }

    public bool Dijkstra(Node startNode, Node targetNode, ref int nodesExplored)
    {
        Heap<Node> openSet = new Heap<Node>(grid3D.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0; // La distancia al principio es 0
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            nodesExplored++;
            closedSet.Add(currentNode);

            if (currentNode == targetNode) return true;

            foreach (Node neighbour in grid3D.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    else openSet.UpdateItem(neighbour);
                }
            }
        }
        return false;
    }

    public bool Greedy(Node startNode, Node targetNode, ref int nodesExplored)
    {
        Heap<Node> openSet = new Heap<Node>(grid3D.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.hCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            nodesExplored++;

            if (currentNode == targetNode)
                return true;

            closedSet.Add(currentNode);

            foreach (Node neighbour in grid3D.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int priority = GetDistance(neighbour, targetNode);

                if (!openSet.Contains(neighbour))
                {
                    neighbour.hCost = priority;
                    neighbour.parent = currentNode;
                    openSet.Add(neighbour);
                }
                else
                {
                    if (priority < neighbour.hCost)
                    {
                        neighbour.hCost = priority;
                        neighbour.parent = currentNode;
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return false;
    }

    public List<Node> FindPath_Nodes(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid3D.NodeFromWorldPoint(startPos);
        Node targetNode = grid3D.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable)
            return null;

        grid3D.ResetGrid();

        int dummy = 0;
        bool success = false;

        switch (currentAlgorithm)
        {
            case AlgorithmType.AStar:
                success = AStar(startNode, targetNode, ref dummy);
                break;
            case AlgorithmType.Dijkstra:
                success = Dijkstra(startNode, targetNode, ref dummy);
                break;
            case AlgorithmType.GreedyBFS:
                success = Greedy(startNode, targetNode, ref dummy);
                break;
            case AlgorithmType.BFS:
                success = BFS(startNode, targetNode, ref dummy);
                break;
        }

        if (!success)
            return null;

        //Devuelve lista de nodos (no solo vectores)
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
}
