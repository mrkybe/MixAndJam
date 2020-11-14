using UnityEngine;

public class GhostScreenShake : MonoBehaviour
{
    [SerializeField]
    private ContactFilter2D contactFilter2D;

    // Start is called before the first frame update
    void Awake()
    {
        results = new RaycastHit2D[40];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private RaycastHit2D[] results;
    private void FixedUpdate()
    {
        float distance = (PlayerController.Instance.transform.position - transform.position).magnitude;
        Ray ray = new Ray(transform.position, (PlayerController.Instance.transform.position - transform.position).normalized);
        var hitCount = Physics2D.Raycast(ray.origin, ray.direction, contactFilter2D, results, distance);

        if (hitCount == 1 || results[0].transform.gameObject == PlayerController.Instance.gameObject)
        {
            PlayerController.Instance.AddGhostlyScreenGhost(gameObject);
        }
        else
        {
            Debug.DrawLine(ray.origin, results[0].point, Color.white);
            Debug.DrawLine(results[0].point, results[1].point, Color.red);
            if (PlayerController.Instance.GhostlyScreenShakeGhosts.Contains(gameObject))
            {
                PlayerController.Instance.RemoveGhostlyScreenGhost(gameObject);
            }
        }
    }
}
