using UnityEngine;

public class Common : MonoBehaviour
{
    public static Common Instance { get; private set; }

    [SerializeField]
    private LayerMask _LayerMaskTerrainAndPlayer;

    public static LayerMask LayerMaskTerrainAndPlayer { get { return Instance._LayerMaskTerrainAndPlayer; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Too Many Commons", this);
        }
    }

    void Start()
    {
        enabled = false;
    }
}
