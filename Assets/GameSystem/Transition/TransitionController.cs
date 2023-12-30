using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Transition
{
    public enum Transition
    {
        BattleEnter
    }

    public enum TransitionState
    {
        None, Start, Pause,
        End
    }

    public class TransitionController : MonoBehaviour
    {
        private static Animator _transitionAnimator;


        public static TransitionState TransitionState { get; private set; } = TransitionState.None;

        public static IEnumerator WaitForTransitionPeak => WaitForTransitionPeakFunc();

        public static IEnumerator WaitForTransitionCompletion => WaitForTransitionCompletionFunc();

        private static IEnumerator StartTransition(Transition transition) {
            if (!_instanceSet) GetInstance();
            _transitionAnimator.Play($"{transition.ToString()}_Start", 0);

            yield return null;
            yield return new WaitUntil(
                () => _transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        private static IEnumerator EndTransition(Transition transition) {
            if (!_instanceSet) GetInstance();
            _transitionAnimator.Play($"{transition.ToString()}_End", 0);

            yield return null;
            yield return new WaitUntil(
                () => _transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public static IEnumerator RunTransitionWithEffect(Transition effect, Action onTransitionPeak = null,
            Action onTransitionFinish = null, float delay = 0.1f) {
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

        public static IEnumerator RunTransition(Transition effect, float delay = 0.1f) {
            TransitionState = TransitionState.Start;
            yield return StartTransition(effect);

            TransitionState = TransitionState.Pause;
            yield return new WaitForSeconds(delay);

            TransitionState = TransitionState.End;
            yield return EndTransition(effect);

            TransitionState = TransitionState.None;
        }

        private static IEnumerator WaitForTransitionPeakFunc() {
            if (TransitionState == TransitionState.End || TransitionState == TransitionState.None)
                throw new MethodAccessException(
                    "No transition in progress, waiting will cause unpredictable delays");

            yield return new WaitUntil(() => TransitionState == TransitionState.Pause);
        }

        private static IEnumerator WaitForTransitionCompletionFunc() {
            yield return new WaitUntil(() => TransitionState == TransitionState.None);
        }

        #region Singleton setup

        private static bool _instanceSet;
        private static bool _createController;

        private static void GetInstance() {
            if (_instanceSet) {
                _instanceSet = true;
                return;
            }

            var instance = new GameObject();
            instance.name = "TransitionController";

            var transitionCanvas = CreateTransitionCanvas();
            transitionCanvas.transform.parent = instance.transform;

            var transitionImage = CreateTransitionImage();
            transitionImage.transform.parent = transitionCanvas.transform;

            var transitionAnimator = transitionCanvas.AddComponent<Animator>();
            transitionAnimator.runtimeAnimatorController =
                UnityEngine.Resources.Load<RuntimeAnimatorController>("Animations/Transition/BattleEnter");

            _transitionAnimator = transitionAnimator;
            _instanceSet = true;
        }

        private static GameObject CreateTransitionCanvas() {
            var transitionCanvas = new GameObject();
            transitionCanvas.name = "Transition_Canvas";

            var canvas = transitionCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 100;

            transitionCanvas.AddComponent<CanvasScaler>();
            transitionCanvas.AddComponent<GraphicRaycaster>();

            return transitionCanvas;
        }

        private static GameObject CreateTransitionImage() {
            var transitionImage = new GameObject();
            transitionImage.name = "Image";
            transitionImage.AddComponent<CanvasRenderer>();

            var image = transitionImage.AddComponent<Image>();
            image.rectTransform.sizeDelta = new Vector2(800, 600);
            image.color = Color.black;

            var canvasGroup = transitionImage.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return transitionImage;
        }

        #endregion
    }
}