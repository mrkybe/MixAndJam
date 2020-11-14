using System.Collections.Generic;
using UnityEngine;

public enum Keycard { LOCKPICK, CROWBAR, TNT }

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

    public List<Keycard> HeldKeycards = new List<Keycard>();

    public List<GameObject> GhostlyScreenShakeGhosts = new List<GameObject>();

    [SerializeField]
    public AudioClip[] PickupSound;

    private AudioSource audioSource;

    Transform ReferenceTransform;
    Camera ReferenceCamera;
    Plane GroundPlane;

    Vector2 result = new Vector2();
    Ray ray;
    float distance;

    private void Awake()
    {
        HeldKeycards = new List<Keycard>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        GroundPlane = new Plane(Vector3.forward, 0);
        if (Instance == null)
        {
            Instance = this;
            ReferenceTransform = Instance.transform;
        }
        else
        {
            Debug.LogError("too many players", this);
        }

        flashLightTransform = transform.Find("Flashlight").transform;
    }

    void Start()
    {
        if (ReferenceCamera == null || ReferenceTransform == null)
        {
            ReferenceCamera = Camera.main;
            ReferenceTransform = Instance.transform;
        }
        controls = ControlFreak.Controls;
        controls.Player.Sprint.started += Sprint_started;
        controls.Player.Sprint.canceled += Sprint_canceled;
        controls.Player.Suicide.performed += Suicide_performed;
    }

    private void Suicide_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Destroy(gameObject);
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
            DeathScreenController.Instance.Gold++;
        }

        if (collision.gameObject.CompareTag("Ghost"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Keycard"))
        {
            Destroy(collision.gameObject);
            audioSource.PlayOneShot(PickupSound.Random());

            var sr = collision.GetComponent<SpriteRenderer>();
            if (sr.sprite.name.ToLower().Contains("lockpick"))
            {
                HeldKeycards.Add(Keycard.LOCKPICK);
                DeathScreenController.Instance.PickedUp(Keycard.LOCKPICK);
            }
            if (sr.sprite.name.ToLower().Contains("crowbar"))
            {
                HeldKeycards.Add(Keycard.CROWBAR);
                DeathScreenController.Instance.PickedUp(Keycard.CROWBAR);
            }
            if (sr.sprite.name.ToLower().Contains("tnt"))
            {
                HeldKeycards.Add(Keycard.TNT);
                DeathScreenController.Instance.PickedUp(Keycard.TNT);
            }
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

    public void OnDestroy()
    {
        DeathScreenController.Instance.PlayerDied();
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
        worldMousePosition = ProcessMouseScreenCoordinates(controls.Player.CursorWorldPosition.ReadValue<Vector2>());
    }

    private Vector2 ProcessMouseScreenCoordinates(Vector2 value)
    {
        ray = ReferenceCamera.ScreenPointToRay(value);
        GroundPlane.Raycast(ray, out distance);
        var pos = ray.GetPoint(distance);
        result.x = pos.x;
        result.y = pos.y;
        return result;
    }
}
