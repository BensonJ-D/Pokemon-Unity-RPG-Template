using System;
using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpWindow : MonoBehaviour
{
    [SerializeField] private Transform window;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject changeLabels;
    [SerializeField] private Text maxHpValue;
    [SerializeField] private Text attackValue;
    [SerializeField] private Text defenceValue;
    [SerializeField] private Text spAtkValue;
    [SerializeField] private Text spDefValue;
    [SerializeField] private Text speedValue;

    private bool WindowOpen { get; set; } = false;
    private Vector3 DefaultPosition { get; set; } = Vector3.zero;
    private Vector3 CanvasOrigin { get; set; }
    
    public void Start()
    {
        DefaultPosition = window.position;
        CanvasOrigin = canvas.transform.position;
    }

    public IEnumerator ShowWindow(Stats before, Stats after)
    {
        yield return ShowWindow(before, after, DefaultPosition);
    }
    
    public IEnumerator ShowWindow(Stats before, Stats after, Vector3 pos)
    {
        window.position = pos;
        changeLabels.SetActive(false);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        SetStatLabels(before);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        changeLabels.SetActive(true);
        SetStatLabels(after - before);
        
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        changeLabels.SetActive(false);
        SetStatLabels(after);
        
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        yield return HideWindow();

    }
    
    public IEnumerator HideWindow()
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.position = CanvasOrigin;
        yield break;
    }

    public void SetStatLabels(Stats stats)
    {
        maxHpValue.text = $"{stats.MaxHp}";
        attackValue.text = $"{stats.Attack}";
        defenceValue.text = $"{stats.Defence}";
        spAtkValue.text = $"{stats.SpAttack}";
        spDefValue.text = $"{stats.SpDefence}";
        speedValue.text = $"{stats.Speed}";
    }
}
