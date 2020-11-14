using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFootprints : MonoBehaviour
{
    [SerializeField]
    GameObject FootprintPrefab;

    [SerializeField]
    float SpawnDistance = 0.2f;

    private Vector3 LastSpawnedPosition;

    [SerializeField]
    public AnimationCurve GhostlyDimmingUnfadeCurve;

    private List<GameObject> activeSteps;

    void Start()
    {
        LastSpawnedPosition = transform.position;
        activeSteps = new List<GameObject>();
    }

    void Update()
    {
        if (Vector3.Distance(LastSpawnedPosition, transform.position) > SpawnDistance)
        {
            var newInstance = Instantiate(FootprintPrefab, transform.position, ExtensionMethods.RotationFromVector((transform.position - LastSpawnedPosition).normalized));
            LastSpawnedPosition = transform.position;
            activeSteps.Add(newInstance);
            SpriteRenderer sr = newInstance.GetComponent<SpriteRenderer>();
            Color c = sr.material.color;
            c.a = 0f;
            sr.material.color = c;

            if (activeSteps.Count > 3)
            {
                var g = activeSteps[0];
                activeSteps.RemoveAt(0);
                StartCoroutine(Unfade(g));
            }
        }
    }

    IEnumerator Unfade(GameObject target)
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        for (float ft = 0f; ft <= GhostlyDimmingUnfadeCurve.keys[GhostlyDimmingUnfadeCurve.length - 1].time; ft += Time.deltaTime)
        {
            Color c = sr.material.color;
            c.a = GhostlyDimmingUnfadeCurve.Evaluate(ft);
            sr.material.color = c;
            yield return null;
        }
        Destroy(target);
    }

    IEnumerator Fade(GameObject target)
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        for (float ft = 2f; ft >= 0; ft -= Time.deltaTime)
        {
            Color c = sr.material.color;
            c.a = ft / 2f;
            sr.material.color = c;
            yield return null;
        }
        Destroy(target);
    }

}
