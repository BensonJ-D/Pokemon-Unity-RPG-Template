using System;
using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle.SubSystems
{
    public class MoveMenu : MonoBehaviour
    {
        [SerializeField] private BattleDialogBox dialogBox;
        [SerializeField] private GameObject childWindow;

        public enum MoveChoice { Move1 = 0, Move2 = 1, Move3 = 2, Move4 = 3, Back = 5 }

        public Dictionary<Participant, MoveChoice> Choice { get; private set; }
        public Dictionary<Participant, SubsystemState> State { get; private set; }

        private List<Move> moves;

        public void Init()
        {
            Choice = new Dictionary<Participant, MoveChoice>
            {
                {Participant.Player, MoveChoice.Move1},
                {Participant.Opponent, MoveChoice.Move1}
            };

            State = new Dictionary<Participant, SubsystemState>
            {
                {Participant.Player, SubsystemState.Closed},
                {Participant.Opponent, SubsystemState.Closed}
            };
        }
        
        public void Reset()
        {
            Choice[Participant.Player] = MoveChoice.Move1;
            Choice[Participant.Opponent] = MoveChoice.Move1;
            State[Participant.Player] = SubsystemState.Closed;
            State[Participant.Opponent] = SubsystemState.Closed;
        }

        public void OpenMenu(Participant participant, List<Move> moveList)
        {
            if (participant == Participant.Player)
            {
                Choice[participant] = Choice[participant] == MoveChoice.Back
                    ? MoveChoice.Move1
                    : Choice[participant];

                childWindow.SetActive(true);
                dialogBox.ClearText();
            }

            moves = moveList;
            State[participant] = SubsystemState.Open;
        }

        public void CloseWindow(Participant participant)
        {
            if (participant == Participant.Player)
            {
                childWindow.SetActive(false);
            }

            State[participant] = SubsystemState.Closed;
        }

        public IEnumerator HandleMoveSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                Choice[participant] = (MoveChoice) Utils.GetGridOption((int) Choice[participant], 2, moves.Count);

                if (Input.GetKeyDown(KeyCode.X))
                {
                    Choice[participant] = MoveChoice.Back;
                    CloseWindow(participant);
                    yield break;
                }

                if (!Input.GetKeyDown(KeyCode.Z)) yield break;
                CloseWindow(participant);
            }
            else
            {
                Choice[participant] = (MoveChoice) Random.Range(0, moves.Count - 1);
                CloseWindow(participant);
            }
        }
    }
}