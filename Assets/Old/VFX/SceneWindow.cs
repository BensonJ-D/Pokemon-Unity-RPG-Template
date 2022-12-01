// using System.Collections;
// using System.Collections.Generic;
// using System.Transition;
// using Battle;
// using UnityEngine;
//
// namespace VFX
// {
//     public class SceneWindow : MonoBehaviour
//     {
//         public Dictionary<Participant, SubsystemState> State { get; private set; }
//         private Scene _parentScene;
//         protected Scene Scene; 
//         
//         public virtual void Init()
//         {
//             // Disable unity MonoBehaviour update events without removing Inspector properties
//             // TODO: Custom inspector for non-MonoBehaviour
//             enabled = false;
//             State = new Dictionary<Participant, SubsystemState> {
//                 {Participant.Player, SubsystemState.Closed}, 
//                 {Participant.Opponent, SubsystemState.Closed}
//             };
//         }
//
//         protected virtual void OnOpen(Participant participant) {
//             State[participant] = SubsystemState.Open;
//             SceneController.Instance.SetActiveScene(Scene);
//         }
//
//         protected virtual void OnClose(Participant participant) {
//             State[participant] = SubsystemState.Closed;
//             SceneController.Instance.SetActiveScene(_parentScene);
//         }
//         
//         public virtual IEnumerator OpenMenu(Participant participant, Scene newParentScene)
//         {
//             _parentScene = newParentScene;
//             yield return TransitionController.Instance.RunTransitionWithEffect(Transition.BattleEnter,
//                 () => OnOpen(participant)
//             );
//         }
//
//         public virtual IEnumerator CloseWindow(Participant participant)
//         {
//             if (participant == Participant.Player)
//             {
//                 yield return TransitionController.Instance.RunTransitionWithEffect(Transition.BattleEnter,
//                     () => OnClose(participant)
//                 );
//             }
//         }
//     }
// }