using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VFX
{
    public enum Transition { BattleEnter }
    public enum TransitionState { None, Start, Pause, End }
    
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Animator transitions;

        public static TransitionState TransitionState { get; set; } = TransitionState.None;


        public IEnumerator StartTransition(Transition transition)
        {
            Debug.Log(transition.ToString());
            transitions.Play($"{transition.ToString()}_Start", 0);

            yield return null;
            yield return new WaitUntil(() => transitions.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public IEnumerator EndTransition(Transition transition)
        {
            transitions.Play($"{transition.ToString()}_End", 0);

            yield return null;
            yield return new WaitUntil(() =>transitions.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public IEnumerator RunTransition(Transition effect, Action OnTransitionPeak = null, 
            Action OnTransitionFinish = null, float delay = 0.1f)
        {
            TransitionState = TransitionState.Start;
            yield return StartTransition(effect);
            
            TransitionState = TransitionState.Pause;
            OnTransitionPeak?.Invoke();
            yield return new WaitForSeconds(delay);
            
            TransitionState = TransitionState.End;
            yield return EndTransition(effect);
        
            OnTransitionFinish?.Invoke();
            TransitionState = TransitionState.None;
        }

        public IEnumerator WaitForTransitionPeak()
        {
            if (TransitionState == TransitionState.End || TransitionState == TransitionState.None)
            {
                throw new MethodAccessException("No transition in progress, waiting will cause unpredictable delays");
            }
            yield return new WaitUntil(() => TransitionState == TransitionState.Pause);
        }
        
        public IEnumerator WaitForTransitionCompletion()
        {
            if (TransitionState == TransitionState.None)
            {
                throw new MethodAccessException("No transition in progress, waiting will cause unpredictable delays");
            }
            yield return new WaitUntil(() => TransitionState == TransitionState.None);
        }
    }
}
