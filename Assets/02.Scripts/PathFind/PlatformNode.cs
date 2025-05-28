using UnityEngine;
using System.Collections.Generic;

public class PlatformNode : MonoBehaviour
{
    public List<PlatformNode> neighbors = new();
    public Vector2 Position => transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);

        Gizmos.color = Color.green;
        foreach (var neighbor in neighbors)
        {
            Gizmos.DrawLine(transform.position, neighbor.Position);
        }
    }
}
