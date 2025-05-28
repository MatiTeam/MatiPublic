using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    public static List<PlatformNode> FindPath(PlatformNode startNode, PlatformNode goalNode)
    {

        if (startNode == null || goalNode == null)
        {
            // Debug.Log("A* 경로 탐색 실패: startNode 또는 goalNode가 null입니다.");
            return null;
        }

        var openSet = new PriorityQueue<PlatformNode>();
        var cameFrom = new Dictionary<PlatformNode, PlatformNode>();
        var gScore = new Dictionary<PlatformNode, float>();
        var fScore = new Dictionary<PlatformNode, float>();

        openSet.Enqueue(startNode, 0);
        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, goalNode);

        while (openSet.Count > 0)
        {
            PlatformNode current = openSet.Dequeue();

            if (current == goalNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (var neighbor in current.neighbors)
            {
                float tentativeGScore = gScore[current] + Vector2.Distance(current.Position, neighbor.Position);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goalNode);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return null; // 경로 없음
    }

    private static float Heuristic(PlatformNode a, PlatformNode b)
    {
        if (a != null && b != null)
        {
            return Vector2.Distance(a.Position, b.Position);
        }
        else
        {
            return float.MaxValue;
        }
    }

    private static List<PlatformNode> ReconstructPath(Dictionary<PlatformNode, PlatformNode> cameFrom, PlatformNode current)
    {
        var path = new List<PlatformNode> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}
