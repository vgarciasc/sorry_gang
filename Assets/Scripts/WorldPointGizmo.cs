using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPointGizmo : MonoBehaviour
{
    [SerializeField]
    Color gizmoColor;

    public void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, new Vector3(0.3f, 0.3f, 0));
    }
}
