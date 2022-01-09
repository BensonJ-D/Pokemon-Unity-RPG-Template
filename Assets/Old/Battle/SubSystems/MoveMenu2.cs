// using System.Collections;
// using System.Collections.Generic;
// using PokemonScripts.Moves;
// using UnityEngine;
//
// namespace Battle.SubSystems
// {
//     public class MoveMenu2 : Window, OptionWindow
//     {
//         [SerializeField] private BattleDialogBox dialogBox;
//         [SerializeField] private GameObject childWindow;
//
//         public enum MoveChoice2 { Move1 = 0, Move2 = 1, Move3 = 2, Move4 = 3, Back = 5 }
//
//         public Dictionary<Participant, MoveChoice2> Choice { get; private set; }
//         public Dictionary<Participant, SubsystemState> State { get; private set; }
//
//         private List<Move> _moves;
//
//         public void Init()
//         {
//             Choice = new Dictionary<Participant, MoveChoice2>
//             {
//                 {Participant.Player, MoveChoice2.Move1},
//                 {Participant.Opponent, MoveChoice2.Move1}
//             };
//
//             State = new Dictionary<Participant, SubsystemState>
//             {
//                 {Participant.Player, SubsystemState.Closed},
//                 {Participant.Opponent, SubsystemState.Closed}
//             };
//         }
//         
//         public void Reset()
//         {
//             Choice[Participant.Player] = MoveChoice2.Move1;
//             Choice[Participant.Opponent] = MoveChoice2.Move1;
//             State[Participant.Player] = SubsystemState.Closed;
//             State[Participant.Opponent] = SubsystemState.Closed;
//         }
//
//         public void OpenMenu(Participant participant, List<Move> moveList)
//         {
//             if (participant == Participant.Player)
//             {
//                 Choice[participant] = Choice[participant] == MoveChoice2.Back
//                     ? MoveChoice2.Move1
//                     : Choice[participant];
//
//                 childWindow.SetActive(true);
//                 dialogBox.ClearText();
//             }
//
//             _moves = moveList;
//             State[participant] = SubsystemState.Open;
//         }
//
//         private void CloseWindow(Participant participant)
//         {
//             if (participant == Participant.Player)
//             {
//                 childWindow.SetActive(false);
//             }
//
//             State[participant] = SubsystemState.Closed;
//         }
//
//         public IEnumerator HandleMoveSelection(Participant participant)
//         {
//             if (participant == Participant.Player)
//             {
//                 Choice[participant] = (MoveChoice2) Utils.GetGridOption((int) Choice[participant], 2, _moves.Count);
//
//                 if (Input.GetKeyDown(KeyCode.X))
//                 {
//                     Choice[participant] = MoveChoice2.Back;
//                     CloseWindow(participant);
//                     yield break;
//                 }
//
//                 if (!Input.GetKeyDown(KeyCode.Z)) yield break;
//                 CloseWindow(participant);
//             }
//             else
//             {
//                 Choice[participant] = (MoveChoice2) Random.Range(0, _moves.Count - 1);
//                 CloseWindow(participant);
//             }
//         }
//     }
// }