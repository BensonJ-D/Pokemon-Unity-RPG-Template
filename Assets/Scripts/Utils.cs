using UnityEngine;

public static class Utils 
{
    public static int GetGridOption(int currentChoice, int columns, int totalOptions)
    {
        var newChoice = currentChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentChoice + columns < totalOptions)
        {
            newChoice = currentChoice + columns;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && currentChoice >= columns)
        {
            newChoice = currentChoice - columns;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentChoice % columns < columns - 1 && currentChoice < totalOptions - 1)
        {
            newChoice = currentChoice + 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentChoice % columns > 0)
        {
            newChoice = currentChoice - 1;
        }

        return newChoice;
    }
    
    public static int GetPokemonOption(int currentChoice, int totalOptions)
    {
        var newChoice = currentChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentChoice < totalOptions - 1)
        {
            newChoice = currentChoice + 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && currentChoice > 0)
        {
            newChoice = currentChoice - 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentChoice == 0)
        {
            newChoice = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentChoice > 0)
        {
            newChoice = 0;
        }

        return newChoice;
    }
}