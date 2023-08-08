using System.Collections;
using System.Collections.Generic;
using System.Transition;
using System.Utilities.Input;
using System.Window.Menu;
using System.Window.Menu.Scroll;
using Characters.Inventory;
using Characters.Monsters;
using Characters.Party.PokemonParty;
using Menus.Inventory.PopupMenu;
using Menus.InventoryMenu.MenuItem;
using Menus.NumberSelector;
using Menus.Party;
using MyBox;
using UnityEngine;

namespace Menus.InventoryMenu
{
    public class InventoryMenu : ScrollMenu<InventoryData>
    {
        [Separator("Inventory Menu Settings")]
        [SerializeField] private ItemDetails itemDetails;
        [SerializeField] private NumberSelectorMenu numberSelector;
        [SerializeField] private InventoryPopupMenu popupMenu;
        [SerializeField] private List<InventoryMenuItem> itemSlots;
        [SerializeField] private PartyMenu partyMenu;

        private Characters.Inventory.Inventory _inventory;
        private PokemonParty _party;

        public override void Initialise()
        {
            base.Initialise();

            OptionMenuItems = new List<IMenuItem<InventoryData>>();
            itemSlots.ForEach(slot => OptionMenuItems.Add(slot));
        }
        
        public IEnumerator OpenWindow(Characters.Inventory.Inventory inventory, PokemonParty party, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
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
            yield return popupMenu.OpenWindow(options, OnPopupConfirm, OnPopupCancel);
            yield return popupMenu.RunWindow();
        }

        private IEnumerator OnPopupConfirm(InventoryPopupMenuOption choice)
        {
            if (choice == InventoryPopupMenuOption.Use)
            {
                StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
                
                yield return TransitionController.Instance.WaitForTransitionPeak();
                yield return partyMenu.OpenWindow(_party, false, OnPartyConfirm, OnPartyCancel);
                yield return popupMenu.CloseWindow();
                
                yield return TransitionController.Instance.WaitForTransitionCompletion();
                yield return partyMenu.RunWindow();
            }
            
            else if (choice == InventoryPopupMenuOption.Toss)
            {
                yield return popupMenu.CloseWindow();
                yield return numberSelector.OpenWindow(1, CurrentOption.Value.quantity, OnTossConfirm, OnTossCancel);
                yield return numberSelector.RunWindow();
            }

            yield return null;
        }

        private IEnumerator OnPopupCancel() => popupMenu.CloseWindow();

        private IEnumerator OnTossConfirm(int numberToToss)
        {
            var item = CurrentOption.Value.item;
            RemoveItemFromInventory(item, numberToToss);

            yield return numberSelector.CloseWindow();
        }

        private IEnumerator OnTossCancel() => numberSelector.CloseWindow();
        
        private IEnumerator OnPartyConfirm(Pokemon choice)
        {
            var item = CurrentOption.Value.item;
            var itemUseValidation = item.ValidateUse(choice);
            
            if (itemUseValidation.Successful) yield return item.OnUse(choice);
            
            yield return partyMenu.TypeMessage(itemUseValidation.ResponseMessage);
            yield return InputController.WaitForConfirm;

            if (!itemUseValidation.Successful) yield break;
                
            if (item.Consumable) RemoveItemFromInventory(item);
            
            yield return OnPartyCancel();
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

        private void RemoveItemFromInventory(Item item, int count = 1)
        {
            _inventory.Remove(item, count);
            UpdateVisibleItems();
            
            if(_inventory.Items.Count == 0) cursor.gameObject.SetActive(false);
        }
    }
}
