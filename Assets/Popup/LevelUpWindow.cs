using System;
using System.Collections;
using System.Collections.Generic;
using System.Utilities.Input;
using System.Window.Menu.Single;
using Characters;
using Characters.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Popup
{ 
    public class LevelUpWindow : SingleWindow
    {
        [SerializeField] private CharacterStats stats;
        [SerializeField] private CharacterStats signedLabels;
        
        public IEnumerator ShowWindow(Stats before, Stats after, Vector3 pos = default)
        {
            yield return base.OpenWindow(pos);

            SetStatLabels(before);
            SetActiveSignedLabels(false); 

            yield return InputController.WaitForConfirm;
            SetActiveSignedLabels(true);
            SetSignedLabels(after - before);
            SetStatLabels(after - before);
        
            yield return new WaitForSeconds(0.5f);
            yield return InputController.WaitForConfirm;
            SetActiveSignedLabels(false); 
            SetStatLabels(after);
        
            yield return new WaitForSeconds(0.5f);
            yield return InputController.WaitForConfirm;
        
            yield return CloseWindow();
        }
        
        private void SetActiveSignedLabels(bool active)
        {
            signedLabels.MaxHealth.gameObject.SetActive(active);
            signedLabels.AttackText.gameObject.SetActive(active);
            signedLabels.DefenceText.gameObject.SetActive(active);
            signedLabels.SpAtkText.gameObject.SetActive(active);
            signedLabels.SpDefText.gameObject.SetActive(active);
            signedLabels.SpeedText.gameObject.SetActive(active);
        }
        
        private void SetSignedLabels(Stats values)
        {
            signedLabels.MaxHealth.text = values.MaxHp > 0 ? "+" : "-";
            signedLabels.AttackText.text = values.Attack > 0 ? "+" : "-";
            signedLabels.DefenceText.text = values.Defence > 0 ? "+" : "-";
            signedLabels.SpAtkText.text = values.SpAttack > 0 ? "+" : "-";
            signedLabels.SpDefText.text = values.SpDefence > 0 ? "+" : "-";
            signedLabels.SpeedText.text = values.Speed > 0 ? "+" : "-";
        }
        
        private void SetStatLabels(Stats values)
        {
            stats.MaxHealth.text = Mathf.Abs(values.MaxHp).ToString();
            stats.AttackText.text = Mathf.Abs(values.Attack).ToString();
            stats.DefenceText.text = Mathf.Abs(values.Defence).ToString();
            stats.SpAtkText.text = Mathf.Abs(values.SpAttack).ToString();
            stats.SpDefText.text = Mathf.Abs(values.SpDefence).ToString();
            stats.SpeedText.text = Mathf.Abs(values.Speed).ToString();
        }
    }
}