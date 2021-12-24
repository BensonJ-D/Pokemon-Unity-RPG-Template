using System.Collections;
using Menu;
using UnityEngine;
using UnityEngine.UI;

public class OptionWindow : Window
{
    #region Singleton setup

    public static OptionWindow Instance { get; private set; }

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
    
    [SerializeField] private GameObject choices;
    [SerializeField] private Label label;
    [SerializeField] private Image cursor;

    private class Option
    {
        public string Value;
        public Transform Transform;
        public Text Text;
    }

    private (int, int) _currentChoice;
    private Option[,] _optionsMatrix;
    private int _optionsRows;
    private int _optionsCols;

    public string Choice { get; private set; }

    public void Start()
    {
        Initiate();
        _optionsMatrix = new Option[,]{};
    }

    public void SetOptions(string[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
    {
        Choice = null;
        _currentChoice = (0, 0);
        _optionsRows = options.GetLength(0);
        _optionsCols = options.GetLength(1);
        _optionsMatrix = new Option[_optionsCols, _optionsRows];
        
        SetSize(width * _optionsCols, height * _optionsRows);
        options.ForEach((y, x, optionText) => AddOption(x, y, optionText, fontSize, spacing));
        
        var cursorPos = _optionsMatrix[0, 0].Transform.localPosition;
        cursorPos.x = -20;
        cursor.transform.localPosition = cursorPos;
    }

    protected override IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
    {
        yield return base.ShowWindow(pos, isCloseable);

        while (Choice == null)
        {
            var updatedChoice = Utils.GetNextMatrixValue(_currentChoice, _optionsMatrix);
            
            _currentChoice = (updatedChoice.Col, updatedChoice.Row);
            var (row, col) = _currentChoice;
            var cursorPos = _optionsMatrix[row, col].Transform.localPosition;
            cursorPos.x -= 20;
            cursor.transform.localPosition = cursorPos;

            if (Input.GetKeyDown(KeyCode.Z)) {
                Choice = _optionsMatrix[row, col].Value;
            } 
            
            if (Input.GetKeyDown(KeyCode.X) && isCloseable) {
                Choice = _optionsMatrix[_optionsRows - 1, _optionsCols - 1].Value;
            }
            yield return null;
        }

        // HideWindow();
        ClearOptions();
    }
    
    private void ClearOptions()
    {
        foreach (var pair in _optionsMatrix) {
            Destroy(pair.Transform.gameObject);
        }
        
        _optionsMatrix = new Option[,]{};
    }

    private void AddOption(int x, int y, string optionText, int fontSize = 45, int spacing = 55)
    {
        if (optionText == null)
        {
            _optionsMatrix[x, y] = null;
            return;
        }

        var newLabel = Instantiate(label, Vector3.zero, Quaternion.identity);
        var labelText = newLabel.text;
        
        var option = new Option{ Value = optionText, Transform = newLabel.transform, Text = labelText };
        _optionsMatrix[x,y] = option;
            
        option.Transform.parent = choices.transform;
        option.Transform.localPosition = new Vector3(x * spacing, (_optionsRows - 1 - y) * spacing);
        option.Transform.localScale = Vector3.one;
        option.Text.text = optionText;
        option.Text.fontSize = fontSize;
    }
}
