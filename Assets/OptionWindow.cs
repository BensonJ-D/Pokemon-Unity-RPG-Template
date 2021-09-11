using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionWindow : Window
{
    #region Singleton setup
    private static OptionWindow _instance;
    public static OptionWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion
    
    [SerializeField] private GameObject choices;
    [SerializeField] private GameObject label;
    [SerializeField] private Image cursor;

    private struct Option
    {
        public string Value;
        public Transform Transform;
        public Text Text;
    }

    private (int, int) _currentChoice;
    private Dictionary<(int, int), Option> _optionsMatrix;
    private int _optionsRows;
    private int _optionsCols;

    public string Choice { get; private set; }

    public void Start()
    {
        Initiate();
        _optionsMatrix = new Dictionary<(int, int), Option>();
    }

    public void SetOptions(string[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
    {
        Choice = null;
        _currentChoice = (0, 0);
        _optionsRows = options.GetLength(0);
        _optionsCols = options.GetLength(1);
        
        SetSize(width * _optionsCols, height * _optionsRows);

        for(var i = 0; i < _optionsRows; i++)
        {
            var newLabel = Instantiate(label, Vector3.zero, Quaternion.identity);
            var labelText = newLabel.GetComponent<Text>();
            
            var option = new Option{ Value = options[i, 0], Transform = newLabel.transform, Text = labelText };
            _optionsMatrix.Add((i, 0), option);
            
            option.Transform.parent = choices.transform;
            option.Transform.localPosition = new Vector3(0, (_optionsRows - 1 - i) * spacing);
            option.Transform.localScale = Vector3.one;
            option.Text.text = options[i, 0];
            option.Text.fontSize = fontSize;
        }

        var cursorPos = _optionsMatrix[(0, 0)].Transform.localPosition;
        cursorPos.x = -20;
        cursor.transform.localPosition = cursorPos;
    }

    public override IEnumerator ShowWindow(bool isCancellable = true)
    {
        yield return ShowWindow(DefaultPosition);
    }

    protected override IEnumerator ShowWindow(Vector2 pos, bool isCancellable = true)
    {
        yield return base.ShowWindow(pos, isCancellable);

        while (Choice == null)
        {
            _currentChoice = Utils.GetGridOption(_currentChoice, _optionsRows, _optionsCols);
            var (row, col) = _currentChoice;
            var cursorPos = _optionsMatrix[(row, col)].Transform.localPosition;
            cursorPos.x = -20;
            cursor.transform.localPosition = cursorPos;

            if (Input.GetKeyDown(KeyCode.Z)) {
                Choice = _optionsMatrix[(row, col)].Value;
            } 
            
            if (Input.GetKeyDown(KeyCode.X) && isCancellable) {
                Choice = _optionsMatrix[(_optionsRows - 1, _optionsCols - 1)].Value;
            }
            yield return null;
        }

        HideWindow();
        ClearOptions();
    }
    
    private void ClearOptions()
    {
        foreach (var pair in _optionsMatrix) {
            Destroy(pair.Value.Transform.gameObject);
        }
        
        _optionsMatrix.Clear();
    }
}
