using System;
using System.Collections;
using System.Window.Menu;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.PartyMenu
{
    [Serializable]
    public class PartyMenuItem : MonoBehaviour, IMenuItem<Pokemon>
    {
        [SerializeField] private Image pokemonIcon;
        [SerializeField] private PartyMenuDetails pokemonDetails;
        [SerializeField] private PartyMenuItemPlate backplate;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip shiftIn;
        [SerializeField] private AnimationClip shiftOut;
        
        public Pokemon Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => null;

        public void SetMenuItem(Pokemon pokemon)
        {
            Value = pokemon;

            if (Value == null) {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                pokemonDetails.SetData(Value); 
                pokemonIcon.sprite = Value.Base.Icon;
            }
        }

        public void SetSelected() => backplate.SetSelected(Value.IsFainted);
        public void SetNotSelected() => backplate.SetNotSelected(Value.IsFainted);
        
        public void SetShiftFrom() => backplate.SetShiftFrom();
        public void SetShiftTo() => backplate.SetShiftTo();
        
        public override string ToString()
        {
            return Value.Name;
        }
        
        public IEnumerator ShiftIn()
        {
            animator.Play(shiftIn.name);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
        
        public IEnumerator ShiftOut()
        {
            animator.Play(shiftOut.name);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
        
        public bool IsNotNullOrEmpty() => Value != null;
    }
}