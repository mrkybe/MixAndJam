using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField]
    TMP_Text DeathText;

    [SerializeField]
    TMP_Text ScoreText;

    [SerializeField]
    TMP_Text InfoText;

    [SerializeField]
    TMP_Text KeycardPickupText;

    [SerializeField]
    TMP_Text KeycardsHeldText;

    public int Gold = 0;

    public static DeathScreenController Instance { get; private set; }

    public string[] DeathTexts = new string[]
    {
        "No one heard your screams...",
        "The screams went unheard...",
        "You scream into the abyss...",
        "Hell and torture await...",
        "You join the eternal patrol...",
        "Doom, gloom, an empty tomb..."
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("too many death screens");
        }

        DeathText.enabled = false;
        ScoreText.enabled = false;
        InfoText.enabled = false;
        KeycardPickupText.enabled = false;
        KeycardsHeldText.text = "";
    }

    public void PlayerDied()
    {
        DeathText.enabled = true;
        DeathText.text = DeathTexts.Random();

        ScoreText.enabled = true;

        ScoreText.text = $"{Gold} Gold Collected";

        InfoText.enabled = true;

        ControlFreak.Controls.UI.RestartLevel.performed += RestartLevel_performed;

        LightFlicker.Instance.DimAll();
    }

    private string lockpick = "\n";
    private string crowbar = "\n";
    private string tnt = "\n";

    public void PickedUp(Keycard keycard)
    {
        switch (keycard)
        {
            case Keycard.LOCKPICK:
                lockpick = "Lockpick\n";
                StartCoroutine(PickupTextFlash("You pickup some lockpicks"));
                break;
            case Keycard.CROWBAR:
                crowbar = "Crowbar\n";
                StartCoroutine(PickupTextFlash("You pickup a crowbar"));
                break;
            case Keycard.TNT:
                tnt = "TNT";
                StartCoroutine(PickupTextFlash("You pickup a bundle of dynamite"));
                break;
        }

        KeycardsHeldText.text = lockpick + crowbar + tnt;
    }

    private IEnumerator PickupTextFlash(string message)
    {
        KeycardPickupText.enabled = true;
        KeycardPickupText.text = message;
        yield return new WaitForSeconds(2f);
        KeycardPickupText.text = "";
        KeycardPickupText.enabled = false;
    }

    private void RestartLevel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
