// using System.Collections;
// using System.Collections.Generic;
// using Battle;
// using Battle.SubSystems.Party;
// using Player;
// using UnityEngine;
// using UnityEngine.UI;
// using VFX;
//
// namespace Inventory
// {
//     public class InventoryMenu : SceneWindow
//     {
//         [SerializeField] private GameObject cursor;
//         [SerializeField] private List<ItemSlot> itemSlots;
//         [SerializeField] private Image icon;
//         [SerializeField] private Text description;
//         [SerializeField] private PartyMenu partyMenu;
//         [SerializeField] private PlayerController player;
//         
//         private int _inventoryPosition;
//         private int _cursorPosition;
//         private int _maxCursorPosition;
//         private int _inventorySize;
//         private Inventory _inventory;
//
//         private static class MenuOptions
//         {
//             public const string 
//                 Use = "USE",
//                 Cancel = "CANCEL";
//         }
//         
//         public override void Init()
//         {
//             Scene = Scene.InventoryView;
//             _cursorPosition = 0;
//             _inventoryPosition = 0;
//             itemSlots.Sort((a, b) => (int)(b.transform.position.y - a.transform.position.y));
//
//             base.Init();
//         }
//
//         // public void Update()
//         // {
//         //     Debug.Log("TEST MESSAGE");
//         // }
//
//         public void SetInventoryData(Inventory newInventory)
//         {
//             _inventory = newInventory;
//             _inventorySize = _inventory.Items.Count;
//             _maxCursorPosition = Mathf.Min(_inventorySize, itemSlots.Count - 1);
//
//             _inventoryPosition = Mathf.Clamp(_inventoryPosition, 0, _inventorySize);
//             _cursorPosition = Mathf.Clamp(_cursorPosition, 0, _maxCursorPosition);
//
//             SetCursorRenderPosition();
//             SetVisibleItems();
//             SetItemDescription();
//         }
//         
//         public IEnumerator HandleItemSelection(Participant participant, bool isCloseable = true)
//         {
//             if (participant == Participant.Player)
//             {
//                 if (Input.GetKeyDown(KeyCode.DownArrow) && _inventoryPosition < _inventorySize)
//                 {
//                     _inventoryPosition = Mathf.Min(_inventoryPosition + 1, _inventorySize);
//                     if (++_cursorPosition > _maxCursorPosition) {
//                         _cursorPosition = _maxCursorPosition;
//                         SetVisibleItems();
//                     }
//
//                     SetCursorRenderPosition();
//                     SetItemDescription();
//                 }
//                 else if (Input.GetKeyDown(KeyCode.UpArrow) && _inventoryPosition > 0)
//                 {
//                     _inventoryPosition = Mathf.Max(0, _inventoryPosition - 1);
//                     if (--_cursorPosition < 0) {
//                         _cursorPosition = 0;
//                         SetVisibleItems();
//                     }
//
//                     SetCursorRenderPosition();
//                     SetItemDescription();
//                 } else if (Input.GetKeyDown(KeyCode.X)) {
//                     yield return CloseWindow(participant);
//                 } else if (Input.GetKeyDown(KeyCode.Z)) {
//                     OptionWindow.Instance.SetOptions(new[,]
//                     {
//                         {MenuOptions.Use},
//                         {MenuOptions.Cancel}
//                     });
//                     yield return OptionWindow.Instance.OpenWindow();
//
//                     if (OptionWindow.Instance.Choice == MenuOptions.Use)
//                     {
//
//                         if (_inventoryPosition == _inventory.Items.Count)
//                         {
//                             yield return CloseWindow(participant);
//                         }
//                         else
//                         {
//                             partyMenu.SetPartyData(player.Party);
//                             yield return partyMenu.OpenMenu(participant, Scene.InventoryView);
//                             while (partyMenu.State[participant] == SubsystemState.Open)
//                             {
//                                 yield return partyMenu.HandlePokemonSelection(participant);
//                             }
//                         }
//                     }
//                 }
//             }
//             
//             yield return null;
//         }
//
//         private void SetVisibleItems()
//         {
//             for (var i = 0; i < itemSlots.Count; i++)
//             {
//                 var itemIndex = _inventoryPosition - _cursorPosition + i;
//                 if (itemIndex == _inventory.Items.Count) {
//                     itemSlots[i].SetCancel();
//                 } else if (itemIndex > _inventory.Items.Count) {
//                     itemSlots[i].SetEmpty();
//                 } else {
//                     itemSlots[i].SetItem(_inventory.Items[itemIndex]);
//                 }
//             }
//         }
//
//         private void SetCursorRenderPosition() {
//             var cursorPos = cursor.transform.position;
//             cursorPos.y = itemSlots[_cursorPosition].transform.position.y;
//             cursor.transform.position = cursorPos;
//         }
//
//         private void SetItemDescription()
//         {
//             description.text = _inventoryPosition < _inventory.Items.Count ? _inventory.Items[_inventoryPosition].item.Description : "";
//         }
//     }
// }
