using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Zoom : MonoBehaviour
{
    private PixelPerfectCamera pixelPerfectCamera;

    // Start is called before the first frame update
    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        ControlFreak.Controls.UI.Zoom.performed += Zoom_performed;
    }

    private void Zoom_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var result = ControlFreak.Controls.UI.Zoom.ReadValue<Vector2>();
        if (result.y > 0)
        {
            pixelPerfectCamera.assetsPPU = Mathf.Min(pixelPerfectCamera.assetsPPU + 5, 200);
        }
        else
        {
            pixelPerfectCamera.assetsPPU = Mathf.Max(pixelPerfectCamera.assetsPPU - 5, 50);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
