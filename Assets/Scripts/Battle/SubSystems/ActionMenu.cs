using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.SubSystems
{
    public class ActionMenu : MonoBehaviour
    {
        [SerializeField] private BattleDialogBox dialogBox;
        [SerializeField] private GameObject childWindow;
        
        public enum ActionChoice { Fight = 0, Bag = 1, Pokemon = 2, Run = 3 }
        public Dictionary<Participant, ActionChoice> Choice { get; private set; }
        public Dictionary<Participant, SubsystemState> State { get; private set; }
        private InputMap Keyboard;
        
        public void Init()
        {
            Keyboard = new InputMap();
            Keyboard.Player.Enable();
            
            Choice = new Dictionary<Participant, ActionChoice> {
                {Participant.Player, ActionChoice.Fight}, 
                {Participant.Opponent, ActionChoice.Fight}
            };
            
            State = new Dictionary<Participant, SubsystemState> {
                {Participant.Player, SubsystemState.Closed}, 
                {Participant.Opponent, SubsystemState.Closed}
            };
        }

        public void Reset()
        {
            Choice[Participant.Player] = ActionChoice.Fight;
            Choice[Participant.Opponent] = ActionChoice.Fight;
            State[Participant.Player] = SubsystemState.Closed;
            State[Participant.Opponent] = SubsystemState.Closed;
        }

        public void OpenMenu(Participant participant, string prompt)
        {
            if (participant == Participant.Player)
            {
                childWindow.SetActive(true);
                dialogBox.SetText(prompt);
            }

            Choice[participant] = ActionChoice.Fight;
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
        
        public IEnumerator HandleActionSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                Choice[participant] = (ActionChoice)Utils.GetGridOption((int) Choice[participant], 2, 2);

                if (!Keyboard.Player.Accept.triggered) yield break;
                CloseWindow(participant);
            }
            else
            {
                Choice[participant] = ActionChoice.Fight;
                CloseWindow(participant);
            }
        }
    }
}