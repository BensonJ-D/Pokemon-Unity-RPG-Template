using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Menu
{
    public abstract class GridMenu<T> : Window
    {
        [SerializeField] private GameObject choices;
        [SerializeField] private Label label;
    
        [Separator]
        [SerializeField] private bool enableHighlight;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color highlightColour;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color fontColour;
    
        [SerializeField] private bool enableCursor;
        [ConditionalField(nameof(enableCursor))] [SerializeField] private MenuCursor cursor;
    
        // [SerializeField] private bool allowEmptyFields; 
        // [ConditionalField(nameof(allowEmptyFields))] [SerializeField] private string emptyFieldTag;
        
        protected IMenuItem<T>[,] OptionsMatrix;
        private (int, int) _currentCursorPosition;
        private IMenuItem<T> _currentOption;

        public IMenuItem<T> Choice { get; private set; }

        protected override IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            if (OptionsMatrix == null) yield break;

            var defaultSelection = OptionsMatrix.GetInitialMatrixPosition(false);
            _currentOption = defaultSelection.Option;
            _currentCursorPosition = (defaultSelection.Col, defaultSelection.Row);
                
            SetDefaultFontColor();
            SetNewHighlightedOption(null, _currentOption);
        
            yield return base.ShowWindow(pos, isCloseable);

            while (WindowOpen) yield return RunWindow();

            ClearOptions();
        }
        
        protected virtual void OnOptionChange(IMenuItem<T> previousOption, IMenuItem<T> newOption)
        {
            var (row, col) = _currentCursorPosition;
            if (enableCursor) SetCursorPosition(row, col);
            if (enableHighlight) SetNewHighlightedOption(previousOption, _currentOption);
        }
        
        protected virtual IEnumerator RunWindow()
        {
            var updatedChoice = OptionsMatrix.GetNextGridMenuOption(_currentCursorPosition);
        
            var previousOption = _currentOption;
            _currentCursorPosition = (updatedChoice.Col, updatedChoice.Row);
            _currentOption = updatedChoice.Option;

            if (previousOption != _currentOption) OnOptionChange(previousOption, _currentOption);

            if (Input.GetKeyDown(KeyCode.Z)) {
                HideWindow(WindowCloseReason.Complete);
            } 
            
            if (Input.GetKeyDown(KeyCode.X) && IsCloseable) {
                HideWindow(WindowCloseReason.Cancel);
            }
            yield return null;
        }

        protected override void OnClose(WindowCloseReason closeReason)
        {
            Choice = closeReason == WindowCloseReason.Complete ? _currentOption : null;
            Debug.Log($"Close reason {closeReason}");
            Debug.Log($"Choice {Choice.ToString()}");
        }
        // public void SetOptions(T[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
        // {
        //     Choice = null;
        //     optionRows = options.GetLength(0);
        //     optionCols = options.GetLength(1);
        //     _optionsMatrix = new MenuOption<T>[optionCols, optionRows];
        //
        //     SetDefaultFontColor();
        //     SetSize(width * optionCols, height * optionRows);
        //     options.ForEach((y, x, option) =>
        //         {
        //             if (!allowEmptyFields && option == null) return;
        //         
        //             AddOption(x, y, option, fontSize, spacing);
        //         }
        //     );
        //
        //     var defaultOption = Utils.GetInitialMatrixPosition(_optionsMatrix, allowEmptyFields);
        //     SetCursorPosition(defaultOption.Col, defaultOption.Row);
        //     SetNewHighlightedOption(null, defaultOption.Option);
        //
        //     _currentOption = defaultOption.Option;
        //     _currentCursorPosition = (defaultOption.Col, defaultOption.Row);
        // }
        
        // private void AddOption(int x, int y, T newOption, int fontSize = 45, int spacing = 55)
        // {
        //     if (newOption == null)
        //     {
        //         _optionsMatrix[x, y] = null;
        //         return;
        //     }
        //
        //     var newLabel = Instantiate(label, Vector3.zero, Quaternion.identity);
        //     var labelText = newLabel.text;
        //
        //     var option = new <T>{ Value = newOption, Transform = newLabel.transform, Text = labelText };
        //     _optionsMatrix[x,y] = option;
        //     
        //     option.Transform.parent = choices.transform;
        //     option.Transform.localPosition = new Vector3(x * spacing, (optionRows - 1 - y) * spacing);
        //     option.Transform.localScale = Vector3.one;
        //     option.Text.text = newOption.ToString();
        //     option.Text.fontSize = fontSize;
        // }
    
        private void ClearOptions()
        {
            foreach (var pair in OptionsMatrix) {
                Destroy(pair.Transform.gameObject);
            }
        
            OptionsMatrix = new IMenuItem<T>[,]{};
        }

        private void SetCursorPosition(int x, int y)
        {
            if (!enableCursor) return;
        
            var cursorPos = OptionsMatrix[x, y].Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }
    
        private void SetDefaultFontColor()
        {
            OptionsMatrix.GetRowsFlattened()
                .ToList()
                .ForEach(value => value.Option.Text.color = fontColour);
        }
    
        private void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next)
        {
            if(!enableHighlight) return;
        
            if(prev != null) prev.Text.color = fontColour;
            if(next != null) next.Text.color = highlightColour;
        }
    }
}