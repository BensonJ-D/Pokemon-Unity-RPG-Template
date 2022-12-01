// using System.Collections;
// using System.Collections.Generic;
// using Battle.SubSystems;
// using PokemonScripts.Moves;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Battle
// {
//     public class BattleDialogBox : MonoBehaviour
//     {
//         [SerializeField] private int lettersPerSecond;
//         [SerializeField] private Text dialogText;
//
//         [SerializeField] private GameObject actionSelector;
//         [SerializeField] private GameObject moveSelector;
//         [SerializeField] private GameObject moveDetails;
//
//         [SerializeField] private List<Text> actionTexts;
//         [SerializeField] private List<Text> moveTexts;
//
//         [SerializeField] private Text ppText;
//         [SerializeField] private Text maxPpText;
//         [SerializeField] private Text typeText;
//
//         private float _lettersPerSecondMultiplier = 1f;
//         private bool _typing;
//         private int _moveChoice;
//         public void Update()
//         {
//             if (Input.GetKeyDown(KeyCode.Z) && _typing)
//             {
//                 _lettersPerSecondMultiplier = 10f;
//             }
//         }
//     
//         public void ClearText() { dialogText.text = ""; }
//         public void SetText(string text) { dialogText.text = text; }
//         public IEnumerator TypeDialog(string dialog)
//         {
//             _lettersPerSecondMultiplier = 1f;
//             dialogText.text = "";
//             yield return new WaitForSeconds(1f / lettersPerSecond);
//         
//             foreach (var letter in dialog.ToCharArray())
//             {
//                 _typing = true;
//                 dialogText.text += letter;
//             
//                 yield return new WaitForSeconds(1f / lettersPerSecond / _lettersPerSecondMultiplier);
//             }
//
//             yield return new WaitForSeconds(1f / _lettersPerSecondMultiplier);
//             _typing = false;
//         }
//
//         public void EnableDialogText(bool enable) => dialogText.enabled = enable;
//         public void EnableActionSelector(bool enable) => actionSelector.SetActive(enable);
//         public void EnableMoveSelector(bool enable)
//         {
//             moveSelector.SetActive(enable);
//             moveDetails.SetActive(enable);
//         }
//     
//         public void UpdateActionSelection(int selectedAction)
//         {
//             foreach (var actionOption in actionTexts)
//             {
//                 actionOption.color = actionTexts.IndexOf(actionOption) == selectedAction ? Color.blue : Color.black;
//             }
//         }
//     
//         public void UpdateMoveSelection(MoveMenu.MoveChoice selectedMove, Move move)
//         {
//             foreach (var moveOption in moveTexts)
//             {
//                 moveOption.color = moveTexts.IndexOf(moveOption) == (int) selectedMove ? Color.blue : Color.black;
//             }
//
//             ppText.text = move.Pp.ToString();
//             maxPpText.text = move.Base.Pp.ToString();
//             typeText.text = move.Base.Type.ToString();
//         }
//
//         public void SetMoveNames(List<Move> moves)
//         {
//             // moveTexts.ForEach(obj => obj.text = moveTexts.IndexOf(obj) < moves.Count ? moves[moveTexts.IndexOf(obj)].Base.Name : "-");
//             for (var i = 0; i < moveTexts.Count; i++)
//             {
//                 moveTexts[i].text = i < moves.Count ? moves[i].Base.Name : "-";
//             }
//         }
//     }
// }
