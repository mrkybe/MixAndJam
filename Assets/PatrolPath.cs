using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PatrolPath : MonoBehaviour
{
    private LineRenderer lineRenderer;

    Vector3[] lineVerts;

    public Vector3[] Waypoints { get; private set; }

    public bool LoopBack;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private static List<PatrolPath> patrolPaths;

    private void Start()
    {
        if (patrolPaths == null)
        {
            patrolPaths = new List<PatrolPath>();
            patrolPaths.Add(this);
        }
        else
        {
            patrolPaths.Add(this);
        }

        Waypoints = new Vector3[transform.childCount + (LoopBack ? 1 : 0)];
        int i = 0;
        foreach (Transform child in transform)
        {
            Waypoints[i] = child.position.WithZ(0);
            i++;
        }
        if (LoopBack)
        {
            Waypoints[i] = Waypoints[0];
        }

        if (!Application.isEditor || Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform NearestWaypoint(Transform t)
    {
        float nearest = float.MaxValue;
        float d = 0;
        Transform dt = transform.GetChild(0);
        Transform result = transform.GetChild(0);
        for (int i = 0; i < transform.childCount; i++)
        {
            dt = transform.GetChild(i);
            d = Vector3.Distance(t.position, dt.position);
            if (d < nearest)
            {
                nearest = d;
                result = dt;
            }
        }
        return result;
    }

    public Transform NextWaypoint(Transform t)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == t && i == transform.childCount - 1)
            {
                return transform.GetChild(0);
            }
            else if (transform.GetChild(i) == t)
            {
                return transform.GetChild(i + 1);
            }
        }
        Debug.LogError("Not found", this);
        return t;
    }

    public Transform PreviousWaypoint(Transform t)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == t && i == 0)
            {
                return transform.GetChild(transform.childCount - 1);
            }
            else if (transform.GetChild(i) == t)
            {
                return transform.GetChild(i - 1);
            }
        }
        Debug.LogError("Not found", this);
        return t;
    }

    // Update is called once per frame
    public void Update()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        lineRenderer.positionCount = transform.childCount + (LoopBack ? 1 : 0);
        lineVerts = new Vector3[transform.childCount + (LoopBack ? 1 : 0)];

        int i = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.name = this.gameObject.name + "_" + (i + 1).ToString("00");
            lineVerts[i] = child.position.WithZ(0.5f);
            child.position = child.position.WithZ(0.5f);
            i++;
        }
        if (LoopBack)
        {
            lineVerts[i] = lineVerts[0];
        }
        lineRenderer.SetPositions(lineVerts);
    }

    public static PatrolPath GetNearest(Transform t)
    {
        if (patrolPaths == null || patrolPaths.Count == 0)
        {
            return null;
        }

        Dictionary<Transform, PatrolPath> pairs = new Dictionary<Transform, PatrolPath>();
        foreach (var path in patrolPaths)
        {
            pairs.Add(path.NearestWaypoint(t), path);
        }

        float nearest = float.MaxValue;
        float d = 0;
        Transform dt = pairs.First().Key;
        Transform result = pairs.First().Key;
        foreach (var kv in pairs)
        {
            dt = kv.Key;
            d = Vector3.Distance(t.position, dt.position);
            if (d < nearest)
            {
                nearest = d;
                result = dt;
            }
        }
        return pairs[result];
    }
}
