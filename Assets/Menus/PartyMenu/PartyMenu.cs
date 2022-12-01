using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transition;
using System.Window;
using System.Window.Dialog;
using System.Window.Menu;
using System.Window.Menu.Grid;
using Characters.Monsters;
using Characters.Party.PokemonParty;
using Menus.Party.Domain;
using Menus.Party.MenuItem;
using Menus.Summary;
using Misc;
using MyBox;
using UnityEngine;

namespace Menus.Party
{ 
    public class PartyMenu : GridMenu<Pokemon>
    {
        [Separator("Party Menu")]
        [SerializeField] private List<PartyMenuItem> menuItems;
        [SerializeField] private PartyPopupMenu popupMenu;
        [SerializeField] private SummaryMenu summaryMenu;
        [SerializeField] private TextBox messageTextBox; 

        public PartyMenuState State { get; private set; }
        private PokemonParty _party;
        private IMenuItem<Pokemon> _switchFrom;
        
        public override void Initialise()
        {
            base.Initialise();
            _switchFrom = null;
            
            OptionsGrid = new IMenuItem<Pokemon>[,]
            {
                {menuItems[0], null, null, null, null, null},
                {null, menuItems[1], menuItems[2], menuItems[3], menuItems[4], menuItems[5]}
            };
        }

        public IEnumerator OpenWindow(PokemonParty pokemon, bool isBattle = false, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            State = isBattle ? PartyMenuState.Battle : PartyMenuState.Normal;
            
            _party = pokemon;

            SetSlots(isBattle ? pokemon.GetCurrentBattleOrder() : pokemon.PartyMembers);

            yield return base.OpenWindow(
                onConfirmCallback: onConfirmCallback ?? DefaultOnConfirmCallback, 
                onCancelCallback: onCancelCallback ?? DefaultOnCancelCallback
            );
        }

        private IEnumerator DefaultOnConfirmCallback(Pokemon choice)
        {
            if (State == PartyMenuState.Switch)
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
                
                _party.SwitchPartyMembers(fromSlot.Value, toSlot.Value);

                Task switchFromAnimateIn = new Task(fromSlot.ShiftIn());
                Task switchToAnimateIn = new Task(toSlot.ShiftIn());
                
                yield return new WaitWhile( () => switchFromAnimateIn.Running || switchToAnimateIn.Running);
                
                toSlot.SetSelected();
                fromSlot.SetNotSelected();

                State = PartyMenuState.Normal;
                yield break;
            }
            
            var options = State == PartyMenuState.Battle 
                ? new List<PartyPopupMenuOption> {PartyPopupMenuOption.Shift, PartyPopupMenuOption.Summary}
                : new List<PartyPopupMenuOption> {PartyPopupMenuOption.Summary, PartyPopupMenuOption.Switch};
            yield return popupMenu.OpenWindow(options, OnPopupConfirm, () => popupMenu.CloseWindow());
            yield return popupMenu.RunWindow();
        }

        private IEnumerator DefaultOnCancelCallback()
        {
            if (State == PartyMenuState.Switch)
            {
                var fromSlot = (PartyMenuItem) _switchFrom;
                var toSlot = (PartyMenuItem) CurrentOption;
                
                fromSlot.SetNotSelected();
                toSlot.SetSelected();

                State = PartyMenuState.Normal;
            }
            else
            {
                CloseReason = WindowCloseReason.Cancel;
                yield return base.OnCancel();
            }
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
                State = PartyMenuState.Switch;
                _switchFrom = CurrentOption;
                ((PartyMenuItem)_switchFrom).SetShiftFrom();

                yield return popupMenu.CloseWindow();
            }
            
            if (choice == PartyPopupMenuOption.Shift)
            {
                yield return popupMenu.CloseWindow();
                
                var pokemon = CurrentOption.Value;
                if (pokemon.IsFainted)
                {
                    yield return messageTextBox.TypeDialog($"{pokemon.Name} has fainted and is unable to battle!");
                } 
                else if ((PartyMenuItem) CurrentOption == menuItems.First())
                {
                    yield return messageTextBox.TypeDialog($"{pokemon.Name} is already in battle!");   
                }
                else
                {
                    Choice = CurrentOption;
                    CloseReason = WindowCloseReason.Complete;
                }
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

            if (State == PartyMenuState.Switch)
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
        
        // ReSharper disable once PossibleNullReferenceException
        private void SetSlots(List<Pokemon> pokemon)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumPokemon = pokemon.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumPokemon.MoveNext();
                enumMenuItems.Current.SetMenuItem(enumPokemon.Current);
                
                if (enumPokemon.Current == null) continue;
                
                enumPokemon.Current.StatusUI = enumMenuItems.Current.PokemonStatus;
                enumMenuItems.Current.SetNotSelected();
            }
        }

        public IEnumerator TypeMessage(string message) => messageTextBox.TypeDialog(message);
    }
}