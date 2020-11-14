using UnityEngine;

public class ControlFreak : MonoBehaviour
{
    public static ControlFreak Instance { get; private set; }
    public static DefaultControls Controls { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Can't have two control freaks");
        }

        if (Controls == null)
        {
            Controls = new DefaultControls();
        }
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }
}
