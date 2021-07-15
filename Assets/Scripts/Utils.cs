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
        input.x = Mathf.RoundToInt(vector.x);
        input.y = input.x != 0 ? 0 : Mathf.RoundToInt(vector.y);
        return input;
    }

    public static int GetGridOption(int currentChoice, int rows, int cols)
    {
        var x = currentChoice % cols;
        var y = currentChoice / rows;
        var input = GetInputVector();
        
        x = Mathf.Clamp(x + input.x, 0, cols - 1);
        y = Mathf.Clamp(y - input.y, 0, rows - 1);
        Debug.Log("row: " + x + ", col: " + y + ", (rows, cols): (" + rows + "," + cols +")");
        return x + y * cols;
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