using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Light2D>().enabled = false;
    }
}
