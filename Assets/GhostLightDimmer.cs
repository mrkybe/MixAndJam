using UnityEngine;

public class GhostLightDimmer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled)
        {
            LightFlicker.Instance.Flick(collision.transform);
        }
    }
}
