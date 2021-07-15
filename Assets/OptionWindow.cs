using System;
using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

public class OptionWindow : MonoBehaviour
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
    
    [SerializeField] private Transform window;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image background;
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

    private bool WindowOpen { get; set; } = false;
    private Vector3 DefaultPosition { get; set; } = Vector3.zero;
    private Vector3 CanvasOrigin { get; set; }

    public string Choice { get; private set; }
    private InputMap Keyboard;
    private Vector2Int arrowInput;
    
    public void Start()
    {
        Keyboard = new InputMap();
        DefaultPosition = window.position;
        CanvasOrigin = canvas.transform.position;
        _optionsMatrix = new Dictionary<(int, int), Option>();
    }

    public void SetOptions(string[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
    {
        Choice = null;
        _currentChoice = (0, 0);
        _optionsRows = options.GetLength(0);
        _optionsCols = options.GetLength(1);
        background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * _optionsCols);
        background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * _optionsRows);
        background.rectTransform.ForceUpdateRectTransforms();

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

        SetCursorPosition(0, 0);
    }

    public IEnumerator ShowWindow(string[,] options, bool isCancellable = true)
    {
        yield return ShowWindow(options, DefaultPosition, isCancellable);
    }
    
    public IEnumerator ShowWindow(string[,] options, Vector2 pos, bool isCancellable = true)
    {
        Keyboard.Player.Enable();
        SetOptions(options);
        
        window.position = pos;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        while (Choice == null)
        {
            var (row, col) = Utils.GetGridOption(_currentChoice, _optionsRows, _optionsCols);
            SetCursorPosition(row, col);

            if (Keyboard.Player.Accept.triggered) {
                Choice = _optionsMatrix[(row, col)].Value;
            } 
            
            if (Keyboard.Player.Cancel.triggered && isCancellable) {
                Choice = _optionsMatrix[(_optionsRows - 1, _optionsCols - 1)].Value;
            }
            yield return null;
        }

        yield return HideWindow();
    }

    private IEnumerator HideWindow()
    {
        Keyboard.Player.Disable();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.position = CanvasOrigin;
        
        foreach (KeyValuePair<(int, int), Option> pair in _optionsMatrix) {
            Destroy(pair.Value.Transform.gameObject);
        }
        
        _optionsMatrix.Clear();
        yield break;
    }

    private void SetCursorPosition(int row, int col)
    {
        var cursorPos = _optionsMatrix[(row, col)].Transform.localPosition;
        cursorPos.x = -20;
        cursor.transform.localPosition = cursorPos;
    }
}
