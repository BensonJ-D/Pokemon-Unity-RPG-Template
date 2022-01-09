using System;
using System.Collections;
using UnityEngine;

namespace VFX
{
    public enum Transition { BattleEnter }
    public enum TransitionState { None, Start, Pause, End }
    
    public class TransitionController : MonoBehaviour
    {        
        #region Singleton setup
        public static TransitionController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } else {
                Instance = this;
            }
        }
        #endregion
        
        [SerializeField] private Animator transitions;
        public static TransitionState TransitionState { get; private set; } = TransitionState.None;

        private IEnumerator StartTransition(Transition transition)
        {
            transitions.Play($"{transition.ToString()}_Start", 0);

            yield return null;
            yield return new WaitUntil(() => transitions.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        private IEnumerator EndTransition(Transition transition)
        {
            transitions.Play($"{transition.ToString()}_End", 0);

            yield return null;
            yield return new WaitUntil(() =>transitions.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public IEnumerator RunTransitionWithEffect(Transition effect, Action onTransitionPeak = null, 
            Action onTransitionFinish = null, float delay = 0.1f)
        {
            TransitionState = TransitionState.Start;
            yield return StartTransition(effect);
            
            TransitionState = TransitionState.Pause;
            // onTransitionPeak?.Invoke();
            yield return new WaitForSeconds(delay);
            
            TransitionState = TransitionState.End;
            yield return EndTransition(effect);
        
            onTransitionFinish?.Invoke();
            TransitionState = TransitionState.None;
        }
        
        public IEnumerator RunTransition(Transition effect, float delay = 0.1f)
        {
            TransitionState = TransitionState.Start;
            yield return StartTransition(effect);
            
            TransitionState = TransitionState.Pause;
            yield return new WaitForSeconds(delay);
            
            TransitionState = TransitionState.End;
            yield return EndTransition(effect);
        
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
            yield return new WaitUntil(() => TransitionState == TransitionState.None);
        }
    }
}
