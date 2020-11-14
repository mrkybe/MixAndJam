using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField]
    private float FlickerSpeed = 5f;

    [SerializeField]
    private float FlickerSize = 0.15f;

    [SerializeField]
    private Color initialColor;

    [SerializeField]
    private float ColorFlickerSize = 0.15f;

    [SerializeField]
    public AnimationCurve GhostlyDimmingUnfadeCurve;

    Transform[] LightsTr;
    Light2D[] Lights;
    float[] ghostlyDimming;

    public static LightFlicker Instance { get; private set; }

    void Start()
    {
        var lites = new List<Light2D>();
        LightsTr = new Transform[transform.childCount];
        int iter = 0;
        foreach (Transform t in transform)
        {
            lites.Add(t.GetComponent<Light2D>());
            LightsTr[iter] = t;
            iter++;
        }

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("too many light flicker", this);
        }

        Lights = lites.ToArray();
        ghostlyDimming = new float[Lights.Length];
        for (int i = 0; i < ghostlyDimming.Length; i++)
        {
            ghostlyDimming[i] = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Lights.Length; i++)
        {
            if (Lights[i] != null)
            {
                var pos = Lights[i].transform.position;
                Lights[i].pointLightOuterRadius = (0.5f + (0.5f - Mathf.PerlinNoise(pos.x * 14.3f + Time.time * FlickerSpeed, pos.y * 7.8f + Time.time * FlickerSpeed)) * FlickerSize) * ghostlyDimming[i];
                Lights[i].intensity = (3.25f + (0.5f - Mathf.PerlinNoise(pos.x * 24.3f + Time.time * FlickerSpeed, pos.y * 17.8f + Time.time * FlickerSpeed))) * ghostlyDimming[i];
                float h, s, v;
                Color.RGBToHSV(initialColor, out h, out s, out v);
                h += (0.5f - Mathf.PerlinNoise(pos.x * 14.3f + Time.time * FlickerSpeed, pos.y * 7.8f + Time.time * FlickerSpeed)) * ColorFlickerSize;
                s += (0.5f - Mathf.PerlinNoise(pos.x * 1.3f + Time.time * FlickerSpeed, pos.y * 777.8f + Time.time * FlickerSpeed)) * ColorFlickerSize;
                Lights[i].color = Color.HSVToRGB(h, s, v);
            }
        }
    }

    public void Flick(Transform g)
    {
        if (GameEnded)
        {
            return;
        }
        for (int i = 0; i < LightsTr.Length; i++)
        {
            if (g == LightsTr[i])
            {
                ghostlyDimming[i] = 0f;
                StartCoroutine(Unfade(i));
                break;
            }
        }
    }

    private bool GameEnded = false;
    public void DimAll()
    {
        StopAllCoroutines();
        for (int i = 0; i < LightsTr.Length; i++)
        {
            StartCoroutine(Fade(i));
        }
        GameEnded = true;
    }

    IEnumerator Fade(int i)
    {
        for (float ft = 0f; ft <= 2f; ft += Time.deltaTime)
        {
            ghostlyDimming[i] = (2f - ft) / 2f;
            yield return null;
        }
    }

    IEnumerator Unfade(int i)
    {
        for (float ft = 0f; ft <= GhostlyDimmingUnfadeCurve.keys[GhostlyDimmingUnfadeCurve.length - 1].time; ft += Time.deltaTime)
        {
            ghostlyDimming[i] = GhostlyDimmingUnfadeCurve.Evaluate(ft);
            yield return null;
        }
        ghostlyDimming[i] = 1f;
    }
}
