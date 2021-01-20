using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private GameObject health;
    [SerializeField] private Text hpLabel;
    [SerializeField] private Text maxHPLabel;

    private Image healthImage;
    private Gradient _gradient;
    private Vector3 prevScale;

    public int HP { get; set; }
    public int MaxHP { get; set; }

    private void Start()
    {
        _gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] colorKey = new GradientColorKey[5];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.2f;
        colorKey[1].color = Color.yellow;
        colorKey[1].time = 0.2f;
        colorKey[2].color = Color.yellow;
        colorKey[2].time = 0.5f;
        colorKey[3].color = new Color(24 / 255f, 173 / 255f, 72 / 255f);
        colorKey[3].time = 0.5f;
        colorKey[4].color = new Color(24 / 255f, 173 / 255f, 72 / 255f);
        colorKey[4].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        _gradient.SetKeys(colorKey, alphaKey);

        hpLabel.text = HP.ToString();
        maxHPLabel.text = MaxHP.ToString();
        var hpNormalise = (float) HP / MaxHP;
        Debug.Log(hpNormalise);
        prevScale = health.transform.localScale = new Vector3(hpNormalise, 1f, 1f);

        healthImage = health.GetComponent<Image>();
        healthImage.color = _gradient.Evaluate(prevScale.x);
    }

    private void Update()
    {
        var scale = health.transform.localScale;

        if (prevScale == scale) return;

        healthImage.color = _gradient.Evaluate(scale.x);
        prevScale = scale;
    }
}