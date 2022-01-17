﻿using System.Collections;
using System.Collections.Generic;
using System.Transition;
using System.Window.Menu;
using System.Window.Menu.Scroll;
using Inventory;
using Menus.Inventory.MenuItem;
using Menus.Inventory.PopupMenu;
using Menus.NumberSelector;
using Menus.Party;
using MyBox;
using PokemonScripts;
using UnityEngine;

namespace Menus.Inventory
{
    public class InventoryMenu : ScrollMenu<InventoryData>
    {
        [Separator("Inventory Menu Settings")]
        [SerializeField] private ItemDetails itemDetails;
        [SerializeField] private NumberSelectorMenu numberSelector;
        [SerializeField] private InventoryPopupMenu popupMenu;
        [SerializeField] private List<InventoryMenuItem> itemSlots;
        [SerializeField] private PartyMenu partyMenu;

        private global::Inventory.Inventory _inventory;
        private PokemonParty _party;

        public void Start()
        {
            base.Initialise();

            OptionMenuItems = new List<IMenuItem<InventoryData>>();
            itemSlots.ForEach(slot => OptionMenuItems.Add(slot));
        }
        
        public IEnumerator OpenWindow(global::Inventory.Inventory inventory, PokemonParty party, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            _party = party;
            _inventory = inventory;
            OptionsList = inventory.Items;
            yield return base.OpenWindow(
                onConfirmCallback: onConfirmCallback ?? DefaultOnConfirmCallback, 
                onCancelCallback: onCancelCallback
            );
        }
        
        private IEnumerator DefaultOnConfirmCallback(InventoryData choice)
        {
            var options = new List<InventoryPopupMenuOption> {InventoryPopupMenuOption.Use, InventoryPopupMenuOption.Toss};
            yield return popupMenu.OpenWindow(options, onConfirmCallback: OnPopupConfirm, onCancelCallback: () => popupMenu.CloseWindow());
            yield return popupMenu.RunWindow();
        }

        private IEnumerator OnPopupConfirm(InventoryPopupMenuOption choice)
        {
            if (choice == InventoryPopupMenuOption.Use)
            {
                StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                
                yield return TransitionController.Instance.WaitForTransitionPeak();
                yield return partyMenu.OpenWindow(_party, onConfirmCallback: OnPartyConfirm, onCancelCallback: OnPartyCancel);
                yield return popupMenu.CloseWindow();
                
                yield return TransitionController.Instance.WaitForTransitionCompletion();
                yield return partyMenu.RunWindow();
            }
            
            else if (choice == InventoryPopupMenuOption.Toss)
            {
                yield return popupMenu.CloseWindow();
                yield return numberSelector.OpenWindow(1, CurrentOption.Value.quantity, OnTossConfirm,
                    () => numberSelector.CloseWindow());
                yield return numberSelector.RunWindow();
            }

            yield return null;
        }

        private IEnumerator OnTossConfirm(int choice)
        {
            InventoryData removalData = new InventoryData(CurrentOption.Value.item, choice);
            _inventory.Remove(removalData);
            SetVisibleItems();
            
            if(_inventory.Items.Count == 0) cursor.gameObject.SetActive(false);

            yield return numberSelector.CloseWindow();
        }
        
        private IEnumerator OnPartyConfirm(Pokemon choice)
        {
            var itemUseValidation = CurrentOption.Value.item.ValidateUse(choice);

            yield return partyMenu.TypeMessage(itemUseValidation.ResponseMessage);
        }
        
        private IEnumerator OnPartyCancel()
        {
            StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return partyMenu.CloseWindow();
                
            yield return TransitionController.Instance.WaitForTransitionCompletion();
        }
        
        protected override void OnOptionChange(IMenuItem<InventoryData> previousOption, IMenuItem<InventoryData> newOption, bool cursorShifted)
        {
            base.OnOptionChange(previousOption, newOption, cursorShifted);
            itemDetails.SetItemDetails(newOption.Value.item);
        }
    }
}
