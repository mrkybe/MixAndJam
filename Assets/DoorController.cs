using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    public Sprite Door_Open;
    public Sprite Door_0;
    public Sprite Door_1;
    public Sprite Door_2;
    public Sprite Door_3;

    [SerializeField]
    public int DoorLevel = 0;

    private SpriteRenderer sr;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] DoorOpenSFX;
    [SerializeField]
    private AudioClip[] DoorCloseSFX;

    private List<GameObject> indoorWay;

    [SerializeField]
    private GameObject colliderObject;

    private bool DoorOpen = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        indoorWay = new List<GameObject>();
        if (DoorLevel == 0)
        {
            sr.sprite = Door_0;
        }
        if (DoorLevel == 1)
        {
            sr.sprite = Door_1;
        }
        if (DoorLevel == 2)
        {
            sr.sprite = Door_2;
        }
        if (DoorLevel == 3)
        {
            sr.sprite = Door_3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Ghost"))
        {
            indoorWay.Add(collision.gameObject);
            Debug.Log("trigger enter");
            if (DoorLevel == 0 && !DoorOpen)
            {
                OpenDoor();
                return;
            }
            if (collision.CompareTag("Player"))
            {
                if (DoorLevel == 1 && !DoorOpen && PlayerController.Instance.HeldKeycards.Contains(Keycard.LOCKPICK))
                {
                    OpenDoor();
                    return;
                }
                if (DoorLevel == 2 && !DoorOpen && PlayerController.Instance.HeldKeycards.Contains(Keycard.CROWBAR))
                {
                    OpenDoor();
                    return;
                }

                if (DoorLevel == 3 && !DoorOpen && PlayerController.Instance.HeldKeycards.Contains(Keycard.TNT))
                {
                    OpenDoor();
                    return;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (indoorWay.Contains(collision.gameObject))
        {
            indoorWay.Remove(collision.gameObject);
            if (indoorWay.Count == 0 && DoorOpen && DoorLevel == 0)
            {
                CloseDoor();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (DoorLevel == 1)
            {
                DeathScreenController.Instance.SayText("I'll need some lockpicks, the key is long gone");
            }
            if (DoorLevel == 2)
            {
                DeathScreenController.Instance.SayText("I'll need a crowbar to pry this open");
            }
            if (DoorLevel == 3)
            {
                DeathScreenController.Instance.SayText("This door way was bricked closed long ago");
            }
        }
    }

    private void OpenDoor()
    {
        sr.sprite = Door_Open;
        DoorOpen = true;
        audioSource.PlayOneShot(DoorOpenSFX.Random());

        try
        {
            colliderObject.GetComponent<BoxCollider2D>().enabled = false;
            colliderObject.GetComponent<ShadowCaster2D>().castsShadows = false;
        }
        catch (Exception)
        {
        }
    }

    private void CloseDoor()
    {
        sr.sprite = Door_0;
        DoorOpen = false;
        audioSource.PlayOneShot(DoorCloseSFX.Random());

        try
        {
            colliderObject.GetComponent<BoxCollider2D>().enabled = true;
            colliderObject.GetComponent<ShadowCaster2D>().castsShadows = true;
        }
        catch (Exception)
        {
        }
    }
}
