using System.Collections;
using System.Utilities.Input;
using GameSystem.Utilities.Tasks;
using TMPro;
using UnityEngine;

namespace GameSystem.Window.Dialog
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private int defaultLettersPerSecond;
        [SerializeField] private float fastForwardMultiplier;
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private RectTransform caret;
        [SerializeField] private Animator caretAnimator;
        [SerializeField] private AnimationClip caretAnimation;

        private bool _fastForward;
        private Task _fastForwardTask;

        public void Resize(Vector2 size) {
            textField.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            textField.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   size.y);
        }

        public void ClearText() {
            textField.text = "";
        }

        public void SetText(string text) {
            textField.text = text;
        }

        public IEnumerator TypeMessage(string text, bool skippable = true) {
            return TypeMessage(text, defaultLettersPerSecond, skippable);
        }

        public IEnumerator TypeMessage(string text, float lettersPerSecond, bool skippable = true) {
            caret.gameObject.SetActive(false);
            var typeDelay = 1f / lettersPerSecond;

            _fastForward = false;
            _fastForwardTask?.Stop();
            _fastForwardTask = new Task(OnFastForwardDown());

            textField.text = "";
            yield return new WaitForSeconds(typeDelay);

            foreach (var letter in text.ToCharArray()) {
                textField.text += letter;
                yield return new WaitForSeconds(typeDelay / (_fastForward ? fastForwardMultiplier : 1f));
            }

            _fastForwardTask?.Stop();
            yield return new WaitForSeconds(typeDelay);
        }

        public IEnumerator TypeDialog(string text) {
            yield return TypeMessage(text, defaultLettersPerSecond);

            var lastCharacter = textField.textInfo.characterCount - 1;
            var characterPosX = textField.textInfo.characterInfo[lastCharacter].bottomRight.x;

            var lastLine = textField.textInfo.lineCount - 1;
            var linePosY = textField.textInfo.lineInfo[lastLine].baseline;

            var rect = caret.rect;
            var posX = characterPosX + rect.width / 2f + 4f;
            var posY = linePosY + rect.height;
            caret.transform.localPosition = new Vector2(posX, posY);
            caret.gameObject.SetActive(true);
            caretAnimator.Play(caretAnimation.name);

            yield return InputController.WaitForConfirm;
            caret.gameObject.SetActive(false);
        }

        private IEnumerator OnFastForwardDown() {
            yield return new WaitUntil(() => InputController.ConfirmOrCancelDown);
            _fastForward = true;
            _fastForwardTask = new Task(OnFastForwardUp());
        }

        private IEnumerator OnFastForwardUp() {
            yield return new WaitUntil(() => InputController.ConfirmAndCancelUp);
            _fastForward = false;
            _fastForwardTask = new Task(OnFastForwardDown());
        }
    }
}