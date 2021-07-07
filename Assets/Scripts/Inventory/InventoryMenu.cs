using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;
using VFX;

namespace Inventory
{
    public class InventoryMenu : MonoBehaviour
    {
        [SerializeField] private GameObject cursor;
        [SerializeField] private List<ItemSlot> itemSlots;
        
        public Dictionary<Participant, SubsystemState> State { get; private set; }

        private int previousOutOfViewItem;
        private int nextOutOfViewItem; 
        private int itemPosition = 0;
        private int cursorPosition = 1;
        private int inventorySize;
        private Inventory inventory;

        public void Init()
        {
            previousOutOfViewItem = 0;
            nextOutOfViewItem = itemSlots.Count + 1;
            itemSlots.Sort((a, b) => (int)(b.transform.position.y - a.transform.position.y));
            
            State = new Dictionary<Participant, SubsystemState> {
                {Participant.Player, SubsystemState.Closed}, 
                {Participant.Opponent, SubsystemState.Closed}
            };
        }
        
        public IEnumerator OpenMenu(Participant participant, Inventory newInventory)
        {
            yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
                OnTransitionPeak: () =>
                {
                    SetInventoryData(newInventory);
                    
                    State[participant] = SubsystemState.Open;
                    SceneController.Instance.SetActiveScene(Scene.InventoryView);
                }
            );
        }

        private void SetInventoryData(Inventory newInventory)
        {
            inventory = newInventory;

            if (itemPosition >= inventory.Items.Count) {
                itemPosition = inventory.Items.Count;
            }

            if (cursorPosition > itemPosition) {
                cursorPosition = itemPosition;
            }

            SetVisibleItems();
        }
        
        public IEnumerator HandleItemSelection(Participant participant, bool isCloseable = true)
        {
            if (participant == Participant.Player)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow) && cursorPosition < nextOutOfViewItem)
                {
                    cursorPosition += 1;
                    itemPosition = Mathf.Min(itemPosition + 1, inventory.Items.Count - 1);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && cursorPosition > previousOutOfViewItem)
                {
                    cursorPosition -= 1;
                    itemPosition = Mathf.Max(0, itemPosition - 1);
                }
                
                if (cursorPosition == previousOutOfViewItem)
                {
                    cursorPosition = 1;
                    SetVisibleItems();
                }
                else if (cursorPosition == nextOutOfViewItem)
                {
                    cursorPosition = nextOutOfViewItem - 1;
                    SetVisibleItems();
                }

                var cursorPos = cursor.transform.position;
                cursorPos.y = itemSlots[cursorPosition - 1].transform.position.y;
                cursor.transform.position = cursorPos;
            }
            
            yield return null;
        }

        private void SetVisibleItems()
        {
            for (var i = 0; i < itemSlots.Count; i++)
            {
                var item = inventory.Items[itemPosition - (cursorPosition - 1) + i];
                if (item == null) {
                    itemSlots[i].SetEmpty();
                } else {
                    itemSlots[i].SetItem(item);
                }
            }
        }
    }
}
