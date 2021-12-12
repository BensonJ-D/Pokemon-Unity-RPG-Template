using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace VFX
{
    public class SceneWindow : MonoBehaviour
    {
        public Dictionary<Participant, SubsystemState> State { get; private set; }
        private Scene parentScene;
        protected Scene Scene; 
        
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
            SceneController.Instance.SetActiveScene(Scene);
        }

        protected virtual void OnClose(Participant participant) {
            State[participant] = SubsystemState.Closed;
            SceneController.Instance.SetActiveScene(parentScene);
        }
        
        public virtual IEnumerator OpenMenu(Participant participant, Scene newParentScene)
        {
            parentScene = newParentScene;
            yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
                onTransitionPeak: () => OnOpen(participant)
            );
        }

        public virtual IEnumerator CloseWindow(Participant participant)
        {
            if (participant == Participant.Player)
            {
                yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
                    onTransitionPeak: () => OnClose(participant)
                );
            }
        }
    }
}