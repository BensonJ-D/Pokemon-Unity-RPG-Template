using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEditor.Callbacks;
using UnityEngine;
using VFX;

namespace DefaultNamespace
{
    public class SceneWindow : MonoBehaviour
    {
        public Dictionary<Participant, SubsystemState> State { get; private set; }
        private Scene source;
        protected Scene self; 
        
        public virtual void Init()
        {
            // Disable unity MonoBehaviour update events without removing Inspector properties
            // TODO: Custom inspector for non-MonoBehaviour
            this.enabled = false;
            State = new Dictionary<Participant, SubsystemState> {
                {Participant.Player, SubsystemState.Closed}, 
                {Participant.Opponent, SubsystemState.Closed}
            };
        }

        protected virtual void OnOpen(Participant participant) {
            State[participant] = SubsystemState.Open;
            SceneController.Instance.SetActiveScene(self);
        }

        protected virtual void OnClose(Participant participant) {
            State[participant] = SubsystemState.Closed;
            SceneController.Instance.SetActiveScene(source);
        }
        
        public virtual IEnumerator OpenMenu(Participant participant, Scene sourceScene)
        {
            source = sourceScene;
            yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
                OnTransitionPeak: () => OnOpen(participant)
            );
        }

        public virtual IEnumerator CloseWindow(Participant participant)
        {
            if (participant == Participant.Player)
            {
                yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
                    OnTransitionPeak: () => OnClose(participant)
                );
            }
        }
    }
}