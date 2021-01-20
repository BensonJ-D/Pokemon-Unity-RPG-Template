using System;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattlePokemon playerUnit;
    [SerializeField] private BattleHud playerHud;
    
    [SerializeField] private BattlePokemon opposingUnit;
    [SerializeField] private BattleHud opposingHud;

    [SerializeField] private BattleDialogBox _dialogBox;
    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        
        opposingUnit.Setup();
        opposingHud.SetData(opposingUnit.Pokemon);
        
        StartCoroutine(_dialogBox.TypeDialog($"A wild {opposingUnit.Pokemon.Base.Name} appeared!"));
    }
}