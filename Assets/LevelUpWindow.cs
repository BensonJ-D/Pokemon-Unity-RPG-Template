using System;
using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpWindow : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Text maxHpValue;
    [SerializeField] private Text attackValue;
    [SerializeField] private Text defenceValue;
    [SerializeField] private Text spAtkValue;
    [SerializeField] private Text spDefValue;
    [SerializeField] private Text speedValue;

    private Vector3 DefaultPosition { get; set; } = Vector3.zero;

    public void Start()
    {
        DefaultPosition = transform.position;
    }

    public IEnumerator ShowWindow(Pokemon before, Pokemon after)
    {
        yield return ShowWindow(before, after, DefaultPosition);
    }
    
    public IEnumerator ShowWindow(Pokemon before, Pokemon after, Vector3 pos)
    {
        transform.position = pos;
        yield break;
    }
}
