using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    public GameObject meshHolder;
    public bool showRays;
    public bool showHits;
    public bool showLines;

    private List<EdgeCollider2D> walls = new List<EdgeCollider2D>();
    private List<Vector2> points = new List<Vector2>();
    private Vector2 mousePosition;
    private List<Vector2> raycastHits;
    private Mesh mesh;

    private bool onTouchDevice = false;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            walls.Add(transform.GetChild(i).GetComponent<EdgeCollider2D>());
        }
        for (int i = 0; i < walls.Count; i++)
        {
            List<Vector2> pointlist = new List<Vector2>(walls[i].points);
            points.AddRange(pointlist.GetRange(0, pointlist.Count-1));
        }

        mesh = new Mesh();
        meshHolder.GetComponent<MeshFilter>().mesh = mesh;

        if (Application.platform == RuntimePlatform.Android)
        {
            onTouchDevice = true;
        }
    }

    private void Update()
    {
        if (onTouchDevice)
        {
            if (Input.touchCount > 0)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            }
        }
        else
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        List<Vector2> hits = Sort(Cast());
        raycastHits = hits;
        if (showLines) { DrawLines(hits); }
        GenerateMesh(hits, ref mesh);
    }

    private void GenerateMesh(List<Vector2> hits, ref Mesh mesh)
    {
        Vector3[] vertices = new Vector3[hits.Count + 1];
        Vector2[] uv = new Vector2[hits.Count + 1];
        int[] triangles = new int[hits.Count * 3];

        vertices[0] = mousePosition;

        for (int x = 0; x < hits.Count; x++)
        {
            vertices[x + 1] = hits[x];
            triangles[x * 3 + 0] = 0;
            triangles[x * 3 + 1] = x + 1;
            if (x < hits.Count - 1)
            {
                triangles[x * 3 + 2] = x + 2;
            }
            else
            {
                triangles[x * 3 + 2] = 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void DrawLines(List<Vector2> pointlist)
    {
        for (int i = 0; i < pointlist.Count; i++)
        {
            Vector2 point = pointlist[i];
            if (i == pointlist.Count-1)
            {
                Debug.DrawLine(point, pointlist[0], Color.red);
            }
            else
            {
                Debug.DrawLine(point, pointlist[i + 1], Color.red);
            }
        }
    }

    private List<Vector2> Cast()
    {
        float offset = 0.1f;
        List<Vector2> hits = new List<Vector2>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            for (int x = -1; x < 2; x++)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, GetDirection(GetAngle(mousePosition, point) + offset * x));
                if (showRays) { Debug.DrawRay(mousePosition, GetDirection(GetAngle(mousePosition, point) + offset * x) * 100); }
                if (hit.collider != null)
                {
                    hits.Add(hit.point);
                }
            }
        }

        return hits;
    }

    private void OnDrawGizmos()
    {
        if (showHits)
        {
            List<Vector2> hits = new List<Vector2> { };
            if (raycastHits != null) { hits = raycastHits; }
            foreach (Vector2 point in hits)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(point, 0.1f);
            }
            foreach (Vector2 point in points)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(point, 0.1f);
            }
        }
    }

    private List<Vector2> Sort(List<Vector2> list)
    {

        if (list.Count < 2) { return list; }
        float pivot = GetAngle(mousePosition, list[Random.Range(0, list.Count - 1)]);
        List<Vector2> smaller = new List<Vector2>();
        List<Vector2> equal = new List<Vector2>();
        List<Vector2> bigger = new List<Vector2>();

        for (int i = 0; i < list.Count; i++)
        {
            float value = GetAngle(mousePosition, list[i]);
            if (value < pivot)
            {
                smaller.Add(list[i]);
            }
            else if (value == pivot)
            {
                equal.Add(list[i]);
            }
            else if (value > pivot)
            {
                bigger.Add(list[i]);
            }
        }
        List<Vector2> result = new List<Vector2>();
        result.AddRange(Sort(bigger));
        result.AddRange(equal);
        result.AddRange(Sort(smaller));
        return result;
    }

    private float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 difference = end - start;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        return angle;
    }

    private Vector2 GetDirection(float angle)
    {
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        return direction;
    }
}
