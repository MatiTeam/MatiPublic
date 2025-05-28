using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.PlayerLoop;

public class PlatformGraphBuilder : MonoBehaviour
{
    [Header("노드 생성 설정")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private float scanSpacing = 1f;
    [SerializeField] private float nodeYOffset = 0.1f;

    [Header("노드 연결 설정")]
    [SerializeField] private float walkableDistance = 2f;
    [SerializeField] private float jumpMaxDistance = 10f;
    [SerializeField] private float jumpMaxHeight = 10f;
    [SerializeField] private float fallMaxDistance = 5f;
    [SerializeField] private float fallHorizontalRange = 1f;

    [Header("플랫폼 타일 타입 필터 (선택)")]
    [SerializeField] private TileBase[] platformTiles; // 비워두면 모든 타일을 노드 생성 대상으로 간주

    private float maxSearchDistance = 2f; // 최대 탐색 거리

    private List<PlatformNode> allNodes = new();
    private List<PlatformNode> endNodes = new();
    private List<PlatformNode> preMapNodes = new();

    public void AddMapNode(List<PlatformNode> nodes)
    {
        preMapNodes = nodes;
    }

    public void BuildGraphFromAllTilemaps()
    {
        foreach (var node in allNodes)
        {
            if (node != null)
            {
                Destroy(node.gameObject);
            }
        }

        allNodes.Clear();
        endNodes.Clear();
        if (preMapNodes != null)
        {
            allNodes.AddRange(preMapNodes);
        }
        preMapNodes = null;
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();

        foreach (var tilemap in allTilemaps)
        {
            if (tilemap.gameObject.layer != LayerMask.NameToLayer("Ground")) continue;

            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            // Debug.Log(tilemap.name + ": " + bounds);

            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y <= bounds.yMax; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(cellPos);

                    if (tile == null) continue;
                    if (platformTiles.Length > 0 && !platformTiles.Contains(tile)) continue;

                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos) + new Vector3(0f, 0.6f, 0f);
                    GameObject nodeObj = Instantiate(nodePrefab, worldPos, Quaternion.identity, transform);
                    PlatformNode node = nodeObj.GetComponent<PlatformNode>();

                    // 겹치는 타일맵 콜라이더가 있는지 확인
                    Collider2D overlap = Physics2D.OverlapCircle(worldPos, 0.1f, LayerMask.GetMask("Ground"));
                    if (overlap == null)
                    {
                        if (tilemap.name == "Platforms01")
                        {
                            Debug.Log("겹침");
                        }
                        Destroy(nodeObj); // 겹치면 제거
                        continue;
                    }

                    allNodes.Add(node);
                }
            }
        }

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        foreach (var node in allNodes)
        {
            int nearbyCount = 0;
            foreach (var other in allNodes)
            {
                if (node == other) continue;
                if (Vector2.Distance(node.Position, other.Position) <= 1.1f)
                {
                    nearbyCount++;
                }
            }

            if (nearbyCount == 1)
            {
                endNodes.Add(node);
            }
        }

        // Debug.Log($"EndNodes 개수: {endNodes.Count}");

        for (int i = 0; i < allNodes.Count; i++)
        {
            for (int j = 0; j < allNodes.Count; j++)
            {
                if (i == j) continue;

                Vector2 from = allNodes[i].Position;
                Vector2 to = allNodes[j].Position;
                float dist = Vector2.Distance(from, to);
                float heightDiff = Mathf.Abs(from.y - to.y);

                // 걷기 연결
                if (dist <= walkableDistance && heightDiff < 0.1f)
                {
                    allNodes[i].neighbors.Add(allNodes[j]);
                }
            }
        }

        // 점프 연결
        foreach (var node in endNodes)
        {
            foreach (var candidate in allNodes)
            {
                if (candidate == node) continue;

                float xDiff = Mathf.Abs(candidate.Position.x - node.Position.x);
                float yDiff = candidate.Position.y - node.Position.y;

                if (xDiff > 0.1f && xDiff < jumpMaxDistance && yDiff > -0.1f && yDiff <= jumpMaxHeight)
                {
                    node.neighbors.Add(candidate);
                }
            }
        }

        // 낙하 연결
        foreach (var node in endNodes)
        {

            foreach (var candidate in allNodes)
            {
                if (candidate == node) continue;

                // x가 거의 동일하고 y는 아래쪽이어야 함
                float xDiff = Mathf.Abs(candidate.Position.x - node.Position.x);
                float yDiff = node.Position.y - candidate.Position.y;

                if (xDiff > 0.1f && xDiff < fallHorizontalRange && yDiff > 0f && yDiff <= fallMaxDistance)
                {
                    node.neighbors.Add(candidate);
                    candidate.neighbors.Add(node);
                }
            }
        }
    }

    public PlatformNode FindClosestNodeTo(Vector2 position)
    {
        PlatformNode closest = null;
        float minDist = float.MaxValue;

        foreach (var node in allNodes)
        {
            float dist = Vector2.Distance(position, node.Position);
            if (dist < minDist && dist <= maxSearchDistance)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}
