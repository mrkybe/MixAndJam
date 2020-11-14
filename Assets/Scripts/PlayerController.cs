using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private DefaultControls controls;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    private Transform flashLightTransform;

    public static PlayerController Instance { get; private set; }

    [SerializeField]
    public float MaxSprintEnergy = 1f;

    [SerializeField]
    public float SprintEnergy = 1f;

    [SerializeField]
    public float MovementSpeed = 1f;

    [SerializeField]
    public float SprintMultiplier = 2f;

    public float RemainingSprintPercent { get => SprintEnergy / MaxSprintEnergy; }

    public bool IsSprinting = false;

    public List<GameObject> GhostlyScreenShakeGhosts = new List<GameObject>();

    [SerializeField]
    public AudioClip[] PickupSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("too many players", this);
        }

        flashLightTransform = transform.Find("Flashlight").transform;
    }

    void Start()
    {
        controls = ControlFreak.Controls;
        controls.Player.Sprint.started += Sprint_started;
        controls.Player.Sprint.canceled += Sprint_canceled;
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        IsSprinting = true;
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        IsSprinting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bonk");
        if (collision.gameObject.CompareTag("PickupDot"))
        {
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(PickupSound.Random());
        }

        if (collision.gameObject.CompareTag("Ghost"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        GetMovementInput();
        GetMouseWorldPosition();

        AimFlashlightAtMouse();
        if (IsSprinting)
        {
            SprintEnergy -= Time.deltaTime;
            if (SprintEnergy <= 0)
            {
                IsSprinting = false;
            }
        }
        else
        {
            if (SprintEnergy < MaxSprintEnergy)
            {
                SprintEnergy += Time.deltaTime / 2f;
            }
        }
        rb.velocity = moveDirection.normalized * MovementSpeed * (IsSprinting ? SprintMultiplier : 1f);
    }
    float closestGhost = float.MaxValue;
    public void AddGhostlyScreenGhost(GameObject ghost)
    {
        if (!GhostlyScreenShakeGhosts.Contains(ghost))
        {
            GhostlyScreenShakeGhosts.Add(ghost);
        }
    }
    public void RemoveGhostlyScreenGhost(GameObject ghost)
    {
        if (GhostlyScreenShakeGhosts.Contains(ghost))
        {
            GhostlyScreenShakeGhosts.Remove(ghost);
        }
    }

    private void AimFlashlightAtMouse()
    {
        flashLightTransform.rotation = ExtensionMethods.RotationFromVector(transform.position - worldMousePosition);
    }

    private void GetMovementInput()
    {
        moveDirection = controls.Player.Movement.ReadValue<Vector2>();
    }

    private Vector3 worldMousePosition = default;
    private void GetMouseWorldPosition()
    {
        worldMousePosition = controls.Player.CursorWorldPosition.ReadValue<Vector2>();
    }
}
