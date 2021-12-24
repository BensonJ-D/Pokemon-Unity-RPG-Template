using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class GridMenu : Window
{
    [SerializeField] private GameObject choices;
    [SerializeField] private Label label;
    
    [Separator]
    [SerializeField] private bool enableHighlight;
    [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color highlightColour;
    [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color fontColour;
    
    [SerializeField] private bool enableCursor;
    [ConditionalField(nameof(enableCursor))] [SerializeField] private MenuCursor cursor;
    
    [SerializeField] private bool allowEmptyFields; 
    [ConditionalField(nameof(allowEmptyFields))] [SerializeField] private string emptyFieldTag; 
    
    [SerializeField] private bool buildFromList; 
    [ConditionalField(nameof(buildFromList))] [SerializeField] private int optionRows; 
    [ConditionalField(nameof(buildFromList))] [SerializeField] private int optionCols; 
    [ConditionalField(nameof(buildFromList))] [SerializeField] private List<string> optionsList; 
    
    private MenuOption<string>[,] _optionsMatrix;
    private (int, int) _currentCursorPosition;
    private MenuOption<string> _currentOption;

    public MenuOption<string> Choice { get; private set; }

    public void Start()
    {
        Initiate();
        SetOptions(new[,]
        {
            {"A", null, "C", null},
            {"A", "B", "C", "D"},
            {"A", null, "C", null},
        });
        
        StartCoroutine(ShowWindow());
    }

    public override IEnumerator ShowWindow(bool isCancellable = true)
    {
        yield return ShowWindow(DefaultPosition);
    }

    protected override IEnumerator ShowWindow(Vector2 pos, bool isCancellable = true)
    {
        if (_optionsMatrix == null) yield break;

        SetDefaultFontColor();
        SetNewHighlightedOption(null, _currentOption);
        
        yield return base.ShowWindow(pos, isCancellable);

        while (Choice == null)
        {
            var updatedChoice = Utils.GetNextMatrixValue(_currentCursorPosition, _optionsMatrix);

            var previousOption = _currentOption;
            _currentCursorPosition = (updatedChoice.Col, updatedChoice.Row);
            _currentOption = updatedChoice.Option;

            if (previousOption != _currentOption)
            {
                var (row, col) = _currentCursorPosition;
                if (enableCursor) SetCursorPosition(row, col);
                if (enableHighlight) SetNewHighlightedOption(previousOption, _currentOption);
            }

            if (Input.GetKeyDown(KeyCode.Z)) {
                Choice = _currentOption;
            } 
            
            if (Input.GetKeyDown(KeyCode.X) && isCancellable) {
                Choice = _optionsMatrix[optionRows - 1, optionCols - 1];
            }
            yield return null;
        }

        HideWindow();
        ClearOptions();
    }
    
    public void SetOptions(string[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
    {
        Choice = null;
        optionRows = options.GetLength(0);
        optionCols = options.GetLength(1);
        _optionsMatrix = new MenuOption<string>[optionCols, optionRows];

        SetDefaultFontColor();
        SetSize(width * optionCols, height * optionRows);
        options.ForEach((y, x, option) =>
            {
                if (!allowEmptyFields && option == null) return;
                
                AddOption(x, y, option, fontSize, spacing);
            }
        );

        var defaultOption = Utils.GetInitialMatrixPosition(_optionsMatrix, allowEmptyFields);
        SetCursorPosition(defaultOption.Col, defaultOption.Row);
        SetNewHighlightedOption(null, defaultOption.Option);

        _currentOption = defaultOption.Option;
        _currentCursorPosition = (defaultOption.Col, defaultOption.Row);
    }
    
    private void AddOption(int x, int y, string newOption, int fontSize = 45, int spacing = 55)
    {
        if (newOption == null)
        {
            _optionsMatrix[x, y] = null;
            return;
        }

        var newLabel = Instantiate(label, Vector3.zero, Quaternion.identity);
        var labelText = newLabel.text;
        
        var option = new MenuOption<string>{ Value = newOption, Transform = newLabel.transform, Text = labelText };
        _optionsMatrix[x,y] = option;
            
        option.Transform.parent = choices.transform;
        option.Transform.localPosition = new Vector3(x * spacing, (optionRows - 1 - y) * spacing);
        option.Transform.localScale = Vector3.one;
        option.Text.text = newOption.ToString();
        option.Text.fontSize = fontSize;
    }
    
    private void ClearOptions()
    {
        foreach (var pair in _optionsMatrix) {
            Destroy(pair.Transform.gameObject);
        }
        
        _optionsMatrix = new MenuOption<string>[,]{};
    }

    private void SetCursorPosition(int x, int y)
    {
        if (!enableCursor) return;
        
        var cursorPos = _optionsMatrix[x, y].Transform.localPosition;
        cursor.SetPosition(cursorPos.x, cursorPos.y);
    }
    
    private void SetDefaultFontColor()
    {
        _optionsMatrix.GetRowsFlattened()
            .ToList()
            .ForEach(value => value.Option.Text.color = fontColour);
    }
    
    private void SetNewHighlightedOption(MenuOption<string> prev, MenuOption<string> next)
    {
        if(!enableHighlight) return;
        
        if(prev != null) prev.Text.color = fontColour;
        if(next != null) next.Text.color = highlightColour;
    }
}