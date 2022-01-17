using System.Collections;
using System.Collections.Generic;
using System.Transition;
using System.Window.Dialog;
using System.Window.Menu;
using System.Window.Menu.Grid;
using Menus.Party.MenuItem;
using Misc;
using MyBox;
using PokemonScripts;
using UnityEngine;

namespace Menus.Party
{ 
    public class PartyMenu : GridMenu<Pokemon>
    {
        [Separator("Party Menu")]
        [SerializeField] private List<PartyMenuItem> menuItems;
        [SerializeField] private PartyPopupMenu popupMenu;
        [SerializeField] private Summary.SummaryMenu summaryMenu;
        [SerializeField] private TextBox messageTextBox; 

        private PokemonParty _party;
        private enum MenuState { Normal, Switch, Battle }
        private MenuState _state;
        private IMenuItem<Pokemon> _switchFrom;
        
        protected override void Initialise()
        {
            base.Initialise();
            _state = MenuState.Normal;
            _switchFrom = null;
            
            OptionsGrid = new IMenuItem<Pokemon>[,]
            {
                {menuItems[0], null, null, null, null, null},
                {null, menuItems[1], menuItems[2], menuItems[3], menuItems[4], menuItems[5]}
            };
        }

        public IEnumerator OpenWindow(PokemonParty pokemon, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            _party = pokemon;
            SetSlots(pokemon.Party);
            yield return base.OpenWindow(
                onConfirmCallback: onConfirmCallback ?? DefaultOnConfirmCallback, 
                onCancelCallback: onCancelCallback ?? DefaultOnCancelCallback
            );
        }

        private IEnumerator DefaultOnConfirmCallback(Pokemon choice)
        {
            if (_state == MenuState.Switch)
            {
                var fromSlot = (PartyMenuItem) _switchFrom;
                var toSlot = (PartyMenuItem) CurrentOption;
                
                if(fromSlot == toSlot) 
                    yield break;

                Task switchFromAnimateOut = new Task(fromSlot.ShiftOut());
                Task switchToAnimateOut = new Task(toSlot.ShiftOut());
                
                yield return new WaitWhile( () => switchFromAnimateOut.Running || switchToAnimateOut.Running);

                var newToPokemon = fromSlot.Value;
                var newFromPokemon = toSlot.Value;
                
                fromSlot.SetMenuItem(newFromPokemon);
                toSlot.SetMenuItem(newToPokemon);
                
                _party.SwitchPokemon(fromSlot.Value, toSlot.Value);

                Task switchFromAnimateIn = new Task(fromSlot.ShiftIn());
                Task switchToAnimateIn = new Task(toSlot.ShiftIn());
                
                yield return new WaitWhile( () => switchFromAnimateIn.Running || switchToAnimateIn.Running);
                
                toSlot.SetSelected();
                fromSlot.SetNotSelected();

                _state = MenuState.Normal;
                yield break;
            }
            
            var options = new List<PartyPopupMenuOption> {PartyPopupMenuOption.Summary, PartyPopupMenuOption.Switch};
            yield return popupMenu.OpenWindow(options, OnPopupConfirm, () => popupMenu.CloseWindow());
            yield return popupMenu.RunWindow();
        }

        private IEnumerator DefaultOnCancelCallback()
        {
            if (_state == MenuState.Switch)
            {
                var fromSlot = (PartyMenuItem) _switchFrom;
                var toSlot = (PartyMenuItem) CurrentOption;
                
                fromSlot.SetNotSelected();
                toSlot.SetSelected();

                _state = MenuState.Normal;
            }

            yield return null;
        }
        
        private IEnumerator OnPopupConfirm(PartyPopupMenuOption choice)
        {
            if (choice == PartyPopupMenuOption.Summary)
            {
                StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                
                yield return TransitionController.Instance.WaitForTransitionPeak();
                yield return summaryMenu.OpenWindow(CurrentOption.Value, onCancelCallback: OnSummaryCancel);
                
                yield return TransitionController.Instance.WaitForTransitionCompletion();
                yield return summaryMenu.RunWindow();
            }
            
            if (choice == PartyPopupMenuOption.Switch)
            {
                _state = MenuState.Switch;
                _switchFrom = CurrentOption;
                ((PartyMenuItem)_switchFrom).SetShiftFrom();

                yield return popupMenu.CloseWindow();
            }
        }

        private IEnumerator OnSummaryCancel()
        {
            StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return summaryMenu.CloseWindow();
                
            yield return TransitionController.Instance.WaitForTransitionCompletion();
        }
        
        protected override void OnOptionChange(IMenuItem<Pokemon> previousOption, IMenuItem<Pokemon> newOption)
        {
            base.OnOptionChange(previousOption, newOption);

            var prev = (PartyMenuItem) previousOption;
            var next = (PartyMenuItem) newOption;

            if (_state == MenuState.Switch)
            {
                if (previousOption == _switchFrom) prev.SetShiftFrom();
                else prev.SetNotSelected();
                
                next.SetShiftTo();
            }
            else
            {
                if(prev != null) prev.SetNotSelected();
                if(next != null) next.SetSelected();
            }
        }

        private void SetSlots(List<Pokemon> pokemon)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumPokemon = pokemon.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumPokemon.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumPokemon.Current);
                if(enumPokemon.Current != null) enumMenuItems.Current.SetNotSelected();
            }
        }

        public IEnumerator TypeMessage(string message) => messageTextBox.TypeDialog(message);
    }
}