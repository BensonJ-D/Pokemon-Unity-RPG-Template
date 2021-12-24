using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public static (int, int) GetGridOption((int, int) currentChoice, int rows, int cols)
    {
        var (row, col) = currentChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            row = Mathf.Min(row + 1, rows - 1);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)){
            row = Mathf.Max(0, row - 1);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)){
            col = Mathf.Min(col + 1, cols - 1);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            col = Mathf.Max(0, col - 1);
        }

        return (row, col);
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

    public struct MatrixValue<T>
    {
        public int Col;
        public int Row;
        public T Option;

        public MatrixValue(int col, int row, T option)
        {
            Col = col;
            Row = row;
            Option = option;
        }
    }

    private static MatrixValue<T>[] GetColumn<T>(this T[,] matrix, int colNumber, bool allowEmptyRows = false)
    {
        var col = Enumerable.Range(0, matrix.GetLength(1))
            .Select(y => new MatrixValue<T>(colNumber, y, matrix[colNumber, y]));
            
        if(allowEmptyRows) return col.ToArray();
            
        return col.Where(value => value.Option != null)
            .ToArray();
    }

    private static MatrixValue<T>[] GetRow<T>(this T[,] matrix, int rowNumber, bool allowEmptyRows = false)
    {
        var row = Enumerable.Range(0, matrix.GetLength(0))
            .Select(x => new MatrixValue<T>(x, rowNumber, matrix[x, rowNumber]));
        
        if(allowEmptyRows) return row.ToArray();
            
        return row.Where(value => value.Option != null)
            .ToArray();
    }
    
    public static MatrixValue<T>[] GetRowsFlattened<T>(this T[,] matrix, bool allowEmptyRows = false)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
            .Select(y => matrix.GetRow(y, allowEmptyRows))
            .SelectMany(item => item)
            .ToArray();
    }
    
    public static MatrixValue<T> GetNextMatrixValue<T>((int, int) currentChoice, T[,] optionsMatrix, bool allowEmptyRows = false)
    {
        var (col, row) = currentChoice;
        var option = optionsMatrix[col, row];
        var originalChoice = new MatrixValue<T>(col, row, option);
        var newChoice = new MatrixValue<T>(col, row, option);
        
        if (!DirectionPressed) return originalChoice;
        
        var curCol = optionsMatrix.GetColumn(col, allowEmptyRows);
        var allRows = GetRowsFlattened(optionsMatrix, allowEmptyRows);
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var orderedColumn = curCol
                .Where(value => value.Row > row)
                .OrderBy(value => value.Row)
                .ToArray();

            var orderedRows = allRows
                .Where(value => value.Row > row)
                .OrderBy(value => value.Row)
                .ThenBy(value => Math.Abs(value.Col - col))
                .ToArray();

            newChoice = orderedColumn.Concat(orderedRows).FirstOrDefault();
        } 
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var orderedColumn = curCol
                .Where(value => value.Row < row)
                .OrderByDescending(value => value.Row)
                .ToArray();

            var orderedRows = allRows
                .Where(value => value.Row < row)
                .OrderByDescending(value => value.Row)
                .ThenBy(value => Math.Abs(value.Col - col))
                .ToArray();

            newChoice = orderedColumn.Concat(orderedRows).FirstOrDefault();
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var orderedRows = allRows
                .Where(value => value.Row >= row)
                .OrderBy(value => value.Row)
                .ThenBy(value => value.Col)
                .ToArray();

            var postSkippedRows = orderedRows
                .SkipWhile(value => value.Col < col)
                .Skip(1)
                .ToArray();
            
            newChoice = postSkippedRows
                .FirstOrDefault();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var orderedRows = allRows
                .Where(value => value.Row <= row)
                .OrderByDescending(value => value.Row)
                .ThenByDescending(value => value.Col)
                .ToArray();
            
            var postSkippedRows = orderedRows
                .SkipWhile(value => value.Col > col)
                .Skip(1)
                .ToArray();
            
            newChoice = postSkippedRows
                .FirstOrDefault();
        }

        if (allowEmptyRows) return newChoice;
        
        return newChoice.Option == null ? originalChoice : newChoice;
    }

    public static void ForEach<T>(this T[,] array, Action<int, int, T> function)
    {
        var maxX = array.GetLength(0);
        var maxY = array.GetLength(1);
        
        Enumerable.Range(0, maxX)
            .Select(x => 
                Enumerable.Range(0, maxY).Select(y => (x, y))
            ).SelectMany(it => it)
            .ToList()
            .ForEach( coord =>
            {
                var (x, y) = coord;
                function(x, y, array[x, y]);
            });
    }

    public static MatrixValue<T> GetInitialMatrixPosition<T>(T[,] array, bool allowEmptyFields)
    {
        var rows = GetRowsFlattened(array);
        return allowEmptyFields ? rows.First() : rows.First(matrixValue => matrixValue.Option != null);
    }

    private static bool DirectionPressed => Input.GetKeyDown(KeyCode.UpArrow) 
                                       || Input.GetKeyDown(KeyCode.LeftArrow)
                                       || Input.GetKeyDown(KeyCode.DownArrow) 
                                       || Input.GetKeyDown(KeyCode.RightArrow);
}