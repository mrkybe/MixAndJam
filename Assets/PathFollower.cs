using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField]
    GameObject Path;

    [SerializeField]
    float Speed = 3f;

    List<Transform> PathNodes;

    private int currentPathNodeIndex = 0;
    private Vector3 targetPos;



    private void Awake()
    {
        PathNodes = new List<Transform>();
    }

    void Start()
    {
        if (PathNodes.Count == 0)
        {
            FillPathNodesFrom(Path);
        }
    }

    private void FillPathNodesFrom(GameObject path)
    {
        foreach (Transform t in Path.transform)
        {
            PathNodes.Add(t);
        }

        targetPos = PathNodes.First().position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * Speed);
        if ((targetPos - transform.position).sqrMagnitude < 0.01f)
        {
            currentPathNodeIndex++;
            if (PathNodes.Count == currentPathNodeIndex)
            {
                currentPathNodeIndex = 0;
            }
            targetPos = PathNodes[currentPathNodeIndex].position;
        }
    }
}
