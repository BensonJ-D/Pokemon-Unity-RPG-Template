using System.Collections;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpWindow : Window
{
    [SerializeField] private GameObject changeLabels;
    [SerializeField] private Text maxHpValue;
    [SerializeField] private Text attackValue;
    [SerializeField] private Text defenceValue;
    [SerializeField] private Text spAtkValue;
    [SerializeField] private Text spDefValue;
    [SerializeField] private Text speedValue;

    public void Start()
    {
        Initiate();
    }
    
    public IEnumerator ShowWindow(Stats before, Stats after)
    {
        yield return ShowWindow(before, after, DefaultPosition);
    }
    
    private IEnumerator ShowWindow(Stats before, Stats after, Vector3 pos)
    {
        yield return base.ShowWindow(pos);

        SetStatLabels(before);
        changeLabels.SetActive(false);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        changeLabels.SetActive(true);
        SetStatLabels(after - before);
        
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        changeLabels.SetActive(false);
        SetStatLabels(after);
        
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        HideWindow();

    }

    private void SetStatLabels(Stats stats)
    {
        maxHpValue.text = $"{stats.MaxHp}";
        attackValue.text = $"{stats.Attack}";
        defenceValue.text = $"{stats.Defence}";
        spAtkValue.text = $"{stats.SpAttack}";
        spDefValue.text = $"{stats.SpDefence}";
        speedValue.text = $"{stats.Speed}";
    }
}