using System;
using System.Linq;
using UnityEngine;

namespace Menu
{
    public class GridMenuOption<T>
    { 
        public readonly int Col;
        public readonly int Row;
        public readonly T Option;

        public GridMenuOption(int col, int row, T option)
        {
            Col = col;
            Row = row;
            Option = option;
        }
    }

    public static class GridMenuExtensionFunctions
    {
        private static GridMenuOption<T>[] GetColumn<T>(this T[,] matrix, int colNumber, bool allowEmptyRows = false)
        {
            var col = Enumerable.Range(0, matrix.GetLength(1))
                .Select(y => new GridMenuOption<T>(colNumber, y, matrix[colNumber, y]));
                
            if(allowEmptyRows) return col.ToArray();
                
            return col.Where(value => value.Option != null)
                .ToArray();
        }

        private static GridMenuOption<T>[] GetRow<T>(this T[,] matrix, int rowNumber, bool allowEmptyRows = false)
        {
            var row = Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => new GridMenuOption<T>(x, rowNumber, matrix[x, rowNumber]));
            
            if(allowEmptyRows) return row.ToArray();
                
            return row.Where(value => value.Option != null)
                .ToArray();
        }
        
        public static GridMenuOption<T>[] GetRowsFlattened<T>(this T[,] matrix, bool allowEmptyRows = false)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(y => matrix.GetRow(y, allowEmptyRows))
                .SelectMany(item => item)
                .ToArray();
        }
        
        public static GridMenuOption<T> GetNextGridMenuOption<T>(this T[,] optionsMatrix, (int, int) currentChoice, bool allowEmptyRows = false)
        {
            var (col, row) = currentChoice;
            var option = optionsMatrix[col, row];
            var originalChoice = new GridMenuOption<T>(col, row, option);
            var newChoice = new GridMenuOption<T>(col, row, option);
            
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
            
            return newChoice == null || newChoice.Option == null ? originalChoice : newChoice;
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

        public static GridMenuOption<T> GetInitialMatrixPosition<T>(this T[,] array, bool allowEmptyFields)
        {
            var rows = GetRowsFlattened(array);
            return allowEmptyFields ? rows.First() : rows.First(matrixValue => matrixValue.Option != null);
        }

        private static bool DirectionPressed => Input.GetKeyDown(KeyCode.UpArrow) 
                                           || Input.GetKeyDown(KeyCode.LeftArrow)
                                           || Input.GetKeyDown(KeyCode.DownArrow) 
                                           || Input.GetKeyDown(KeyCode.RightArrow);
    }
}