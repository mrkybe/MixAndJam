#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridPickupPlacer : MonoBehaviour
{
    public string colliderTag = "GenerateShadowCasters";
    public GameObject shadowCasterPrefab;
    public Transform pickupsHolder;
    public bool removePreviouslyGenerated = true;

    public float gridScale = 0.64f;

    bool[,] emptySpots;
    GameObject[,] instances;

    public GameObject[] Generate()
    {
        Debug.Log("### Generating ShadowCasters ###");

        /* get the bounds of the area to check */

        // collect colliders specified by tag

        var colliders = new List<Collider2D>();
        var tagedGos = GameObject.FindGameObjectsWithTag(colliderTag);

        foreach (var go in tagedGos)
        {
            var goColliders = go.GetComponents<Collider2D>();

            foreach (var goc in goColliders)
            {
                colliders.Add(goc);
            }
        }

        if (colliders.Count == 0)
        {
            Debug.Log("No colliders found, aborting.");
            return new GameObject[0];
        }

        // get outer-most bound vertices, defining the area to check

        var bottomLeft = new Vector2(Mathf.Infinity, Mathf.Infinity);
        var topRight = new Vector2(-Mathf.Infinity, -Mathf.Infinity);

        foreach (var col in colliders)
        {
            bottomLeft.x = Mathf.Min(bottomLeft.x, col.bounds.min.x);
            bottomLeft.y = Mathf.Min(bottomLeft.y, col.bounds.min.y);
            topRight.x = Mathf.Max(topRight.x, col.bounds.max.x);
            topRight.y = Mathf.Max(topRight.y, col.bounds.max.y);
        }

        Debug.Log("Bounds: downLeft = (" + bottomLeft.x + ", " + bottomLeft.y + ")");
        Debug.Log("Bounds: topRight = (" + topRight.x + ", " + topRight.y + ")");

        /* check the area for collisions */

        var countX = Mathf.RoundToInt((topRight.x - bottomLeft.x) / gridScale);
        var countY = Mathf.RoundToInt((topRight.y - bottomLeft.y) / gridScale);

        emptySpots = new bool[countX, countY];
        instances = new GameObject[countX, countY];

        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                emptySpots[x, y] = !IsHit(new Vector2(bottomLeft.x + (x * gridScale) + (0.5f * gridScale),
                                               bottomLeft.y + (y * gridScale) + (0.5f * gridScale)));
            }
        }

        /* instantiate shadow casters, merging single tiles horizontaly */

        // removing old shadow casters! careful!

        if (removePreviouslyGenerated)
        {
            foreach (Transform pickup in pickupsHolder)
            {
                DestroyImmediate(pickup.gameObject);
            }
        }

        // create new ones

        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                if (emptySpots[x, y])
                {
                    var currentInstance = (GameObject)PrefabUtility.InstantiatePrefab(shadowCasterPrefab, pickupsHolder);
                    currentInstance.transform.position = new Vector3(bottomLeft.x + (x * gridScale) + (0.5f * gridScale),
                                                                     bottomLeft.y + (y * gridScale) + (0.5f * gridScale), 0.0f);
                }
            }
        }

        Debug.Log("ShadowCasters generated.");

        /* return shadow casters */
        var pickupInstances = new List<GameObject>();

        for (int y = 0; y < countY; y++)
        {
            for (int x = 0; x < countX; x++)
            {
                if (instances[x, y] != null && !pickupInstances.Contains(instances[x, y]))
                {
                    pickupInstances.Add(instances[x, y]);
                }
            }
        }

        return pickupInstances.ToArray();
    }

    bool IsHit(Vector2 pos)
    {
        var margin = .2f; // prevents overlapping

        // get tile bounds

        var bottomLeft = new Vector2(pos.x - (0.5f * gridScale) + margin, pos.y + (0.5f * gridScale) - margin);
        var topRight = new Vector2(pos.x + (0.5f * gridScale) - margin, pos.y - (0.5f * gridScale) + margin);

        //check for collisions

        Collider2D[] colliders = Physics2D.OverlapAreaAll(bottomLeft, topRight);

        foreach (var col in colliders)
        {
            if (col.CompareTag(colliderTag))
            {
                return true;
            }
        }

        return false;
    }
}

#endif
