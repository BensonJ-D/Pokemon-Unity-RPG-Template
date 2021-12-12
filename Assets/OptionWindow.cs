using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject label;
    [SerializeField] private Image cursor;

    private struct Option
    {
        public string Value;
        public Transform Transform;
        public Text Text;
    }

    private (int, int) currentChoice;
    private Dictionary<(int, int), Option> optionsMatrix;
    private int optionsRows;
    private int optionsCols;

    public string Choice { get; private set; }

    public void Start()
    {
        Initiate();
        optionsMatrix = new Dictionary<(int, int), Option>();
    }

    public void SetOptions(string[,] options, int width = 300, int height = 60, int fontSize = 45, int spacing = 55)
    {
        Choice = null;
        currentChoice = (0, 0);
        optionsRows = options.GetLength(0);
        optionsCols = options.GetLength(1);
        
        SetSize(width * optionsCols, height * optionsRows);

        for(var i = 0; i < optionsRows; i++)
        {
            var newLabel = Instantiate(label, Vector3.zero, Quaternion.identity);
            var labelText = newLabel.GetComponent<Text>();
            
            var option = new Option{ Value = options[i, 0], Transform = newLabel.transform, Text = labelText };
            optionsMatrix.Add((i, 0), option);
            
            option.Transform.parent = choices.transform;
            option.Transform.localPosition = new Vector3(0, (optionsRows - 1 - i) * spacing);
            option.Transform.localScale = Vector3.one;
            option.Text.text = options[i, 0];
            option.Text.fontSize = fontSize;
        }

        var cursorPos = optionsMatrix[(0, 0)].Transform.localPosition;
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
            currentChoice = Utils.GetGridOption(currentChoice, optionsRows, optionsCols);
            var (row, col) = currentChoice;
            var cursorPos = optionsMatrix[(row, col)].Transform.localPosition;
            cursorPos.x = -20;
            cursor.transform.localPosition = cursorPos;

            if (Input.GetKeyDown(KeyCode.Z)) {
                Choice = optionsMatrix[(row, col)].Value;
            } 
            
            if (Input.GetKeyDown(KeyCode.X) && isCancellable) {
                Choice = optionsMatrix[(optionsRows - 1, optionsCols - 1)].Value;
            }
            yield return null;
        }

        HideWindow();
        ClearOptions();
    }
    
    private void ClearOptions()
    {
        foreach (var pair in optionsMatrix) {
            Destroy(pair.Value.Transform.gameObject);
        }
        
        optionsMatrix.Clear();
    }
}
