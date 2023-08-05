using System.Collections;
using UnityEngine;

namespace System.Window.Dialog
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private int defaultLettersPerSecond;
        [SerializeField] private UnityEngine.UI.Text dialogText;

        private float _lettersPerSecondMultiplier = 1f;
        private bool _typing;
        private bool _skippable;
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z) && _typing && _skippable)
            {
                _lettersPerSecondMultiplier = 10f;
            }
        }
    
        public void ClearText() { dialogText.text = ""; }
        public void SetText(string text) { dialogText.text = text; }

        public IEnumerator TypeDialog(string dialog, bool skippable = true) => TypeDialog(dialog, defaultLettersPerSecond, skippable);
        public IEnumerator TypeDialog(string dialog, float lettersPerSecond, bool skippable = true)
        {
            _skippable = skippable;
            _lettersPerSecondMultiplier = 1f;
            dialogText.text = "";
            yield return new WaitForSeconds(1f / lettersPerSecond);
        
            foreach (var letter in dialog.ToCharArray())
            {
                _typing = true;
                dialogText.text += letter;
            
                yield return new WaitForSeconds(1f / lettersPerSecond / _lettersPerSecondMultiplier);
            }

            yield return new WaitForSeconds(1f / _lettersPerSecondMultiplier);
            _typing = false;
        }
    }
}