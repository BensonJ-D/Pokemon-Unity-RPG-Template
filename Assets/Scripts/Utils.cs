using System;
using UnityEngine;

public static class Utils 
{
    private static InputMap Keyboard = new InputMap();
    
    public static Vector3Int GetInputVector()
    {
        Keyboard.Player.Enable();
        Vector3Int input = Vector3Int.zero;
        var vector = Keyboard.Player.Move.ReadValue<Vector2>();
        Debug.Log(vector);
        input.x = Mathf.RoundToInt(vector.x);
        input.y = input.x != 0 ? 0 : Mathf.RoundToInt(vector.y);
        return input;
    }
    
    public static int GetGridOption(int currentChoice, int columns, int totalOptions)
    {
        var input = GetInputVector();
        var newChoice = currentChoice + input.x + (columns * input.y);
        
        return Mathf.Clamp(newChoice, 0, totalOptions);
    }
    
    public static (int, int) GetGridOption((int, int) currentChoice, int rows, int cols)
    {
        var (row, col) = currentChoice;
        var input = GetInputVector();
        
        row = Mathf.Clamp(row + input.x, 0, rows - 1);
        col = Mathf.Clamp(col + input.y, 0, cols - 1);

        return (row, col);
    }

    public static int GetPokemonOption(int currentChoice, int totalOptions)
    {
        var newChoice = currentChoice;
        var input = GetInputVector();

        if (input.x != 0) {
            newChoice = input.x == 1 ? Mathf.Min( 0, totalOptions - 1) : 0;
        } else {
            newChoice = Mathf.Clamp(newChoice + input.y, 0, totalOptions - 1);
        }
        
        return newChoice;
    }
}