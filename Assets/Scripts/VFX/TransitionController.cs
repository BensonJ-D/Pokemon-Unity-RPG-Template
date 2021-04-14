using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VFX
{
    public enum CanvasView { PartyView, BattleView }
    public enum Transition { BattleEnter }
    public enum TransitionState { None, Start, Pause, End }
    
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private List<Canvas> views;
        [SerializeField] private Animator transitions;

        private Stack<CanvasView> sceneHistory = new Stack<CanvasView>();

        public void Awake()
        {
            foreach (var canvas in views)
            {
                canvas.enabled = false;
            }
        }

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
        
        public IEnumerator MoveToScreen(Transition effect, CanvasView newScene, float delay = 0.1f)
        {
            GameController.TransitionState = TransitionState.Start;
            yield return StartTransition(effect);
            
            if (sceneHistory.Count > 0)
            {
                viewCamera.enabled = true;
                var oldScene = sceneHistory.First();
                views.Find(view => view.name == $"{oldScene}_Canvas").enabled = false;
            }
            
            viewCamera.enabled = true;
            sceneHistory.Push(newScene);
            Canvas newView = views.Find(view => view.name == $"{newScene}_Canvas");
            newView.enabled = true;
            newView.renderMode = RenderMode.ScreenSpaceCamera;
            
            GameController.TransitionState = TransitionState.Pause;
            yield return new WaitForSeconds(delay);
            
            GameController.TransitionState = TransitionState.End;
            yield return EndTransition(effect);
        
            GameController.TransitionState = TransitionState.None;
        }
    }
}
