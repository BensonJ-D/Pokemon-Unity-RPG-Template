using System;
using System.Linq;
using UnityEngine;

namespace GameSystem.Window.Menu.Grid
{
    public class GridMenuOption<T>
    {
        public readonly int Col;
        public readonly IMenuItem<T> Option;
        public readonly int Row;

        public GridMenuOption(int col, int row, IMenuItem<T> option) {
            Col = col;
            Row = row;
            Option = option;
        }

        public int FlattenedColumnId(int colSize) {
            return Row + Col * colSize;
        }

        public int FlattenedRowId(int rowSize) {
            return Col + Row * rowSize;
        }
    }

    public static class GridMenuExtensionFunctions
    {
        private static GridMenuOption<T>[] GetColumn<T>(this IMenuItem<T>[,] matrix, int colNumber,
            bool allowEmptyRows = false) {
            var col = Enumerable.Range(0, matrix.GetLength(1))
                .Select(y => new GridMenuOption<T>(colNumber, y, matrix[colNumber, y]));

            if (allowEmptyRows) return col.ToArray();

            return col.Where(value => value.Option != null && value.Option.IsNotNullOrEmpty())
                .ToArray();
        }

        private static GridMenuOption<T>[] GetRow<T>(this IMenuItem<T>[,] matrix, int rowNumber,
            bool allowEmptyRows = false) {
            var row = Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => new GridMenuOption<T>(x, rowNumber, matrix[x, rowNumber]));

            if (allowEmptyRows) return row.ToArray();

            return row.Where(value => value.Option != null && value.Option.IsNotNullOrEmpty())
                .ToArray();
        }

        private static GridMenuOption<T>[] GetRowsFlattened<T>(this GridMenu<T> gridMenu, bool allowEmptyRows = false) {
            var grid = gridMenu.OptionsGrid;
            return grid.GetRowsFlattened();
        }

        public static GridMenuOption<T>[]
            GetRowsFlattened<T>(this IMenuItem<T>[,] matrix, bool allowEmptyRows = false) {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(y => matrix.GetRow(y, allowEmptyRows))
                .SelectMany(item => item)
                .ToArray();
        }

        public static GridMenuOption<T>[]
            GetColumnsFlattened<T>(this GridMenu<T> gridMenu, bool allowEmptyRows = false) {
            var grid = gridMenu.OptionsGrid;
            return Enumerable.Range(0, grid.GetLength(0))
                .Select(x => grid.GetColumn(x, allowEmptyRows))
                .SelectMany(item => item)
                .ToArray();
        }

        public static GridMenuOption<T> GetNextGridMenuOption<T>(this GridMenu<T> gridMenu, Vector2Int inputDirection,
            bool allowEmptyRows = false) {
            var (col, row) = gridMenu.CurrentCursorPosition;

            var optionMenuItems = gridMenu.OptionsGrid;
            var option = gridMenu.CurrentOption;

            var originalChoice = new GridMenuOption<T>(col, row, option);
            var newChoice = new GridMenuOption<T>(col,      row, option);

            if (inputDirection == Vector2Int.zero) return originalChoice;

            var rowSize = gridMenu.OptionsGrid.GetLength(0);
            var colSize = gridMenu.OptionsGrid.GetLength(1);

            var curCol = optionMenuItems.GetColumn(col, allowEmptyRows);
            var curRow = optionMenuItems.GetRow(row, allowEmptyRows);

            var allRows = gridMenu.GetRowsFlattened(allowEmptyRows);
            var allCols = gridMenu.GetColumnsFlattened(allowEmptyRows);

            if (inputDirection.y < 0) {
                var orderedCol = curCol
                    .Where(value => value.Row > row)
                    .OrderBy(value => value.Row)
                    .ToArray();

                var orderedColumns = allCols
                    .Where(value => value.Row > row)
                    .Where(value => value.FlattenedColumnId(colSize) < originalChoice.FlattenedColumnId(colSize))
                    .OrderByDescending(value => value.FlattenedColumnId(colSize))
                    .ToArray();

                var orderedRows = allRows
                    .Where(value => value.Row > row)
                    .OrderBy(value => value.Row)
                    .ThenBy(value => Math.Abs(value.Col - col))
                    .ToArray();

                newChoice = orderedCol.Concat(orderedColumns).Concat(orderedRows).FirstOrDefault();
                if (newChoice != null) {
                    curCol = optionMenuItems.GetColumn(newChoice.Col, allowEmptyRows);
                    curRow = optionMenuItems.GetRow(newChoice.Row, allowEmptyRows);
                }
            }

            if (inputDirection.y > 0) {
                var orderedCol = curCol
                    .Where(value => value.Row < row)
                    .OrderByDescending(value => value.Row)
                    .ToArray();

                var orderedColumns = allCols
                    .Where(value => value.Row < row)
                    .Where(value => value.FlattenedColumnId(colSize) < originalChoice.FlattenedColumnId(colSize))
                    .OrderByDescending(value => value.FlattenedColumnId(colSize))
                    .ToArray();

                var orderedRows = allRows
                    .Where(value => value.Row < row)
                    .OrderBy(value => value.Row)
                    .ThenBy(value => Math.Abs(value.Col - col))
                    .ToArray();

                newChoice = orderedCol.Concat(orderedColumns).Concat(orderedRows).FirstOrDefault();
                if (newChoice != null) curRow = optionMenuItems.GetRow(newChoice.Row, allowEmptyRows);
            }

            if (inputDirection.x > 0) {
                var orderedRow = curRow
                    .Where(value => value.Col > col)
                    .OrderBy(value => value.Col)
                    .ToArray();

                var orderedRows = allRows
                    .Where(value => value.Col > col)
                    .Where(value => value.FlattenedRowId(rowSize) < originalChoice.FlattenedRowId(rowSize))
                    .OrderByDescending(value => value.FlattenedRowId(rowSize))
                    .ToArray();

                var orderedColumns = allCols
                    .Where(value => value.Col > col)
                    .OrderBy(value => value.Col)
                    .ThenBy(value => Math.Abs(value.Row - row))
                    .ToArray();

                newChoice = orderedRow.Concat(orderedRows).Concat(orderedColumns).FirstOrDefault();
                if (newChoice != null) curRow = optionMenuItems.GetRow(newChoice.Row, allowEmptyRows);
            }

            if (inputDirection.x < 0) {
                var orderedRow = curRow
                    .Where(value => value.Col < col)
                    .OrderByDescending(value => value.Col)
                    .ToArray();

                var orderedRows = allRows
                    .Where(value => value.Col < col)
                    .Where(value => value.FlattenedRowId(rowSize) < originalChoice.FlattenedRowId(rowSize))
                    .OrderByDescending(value => value.FlattenedRowId(rowSize))
                    .ToArray();

                var orderedColumns = allCols
                    .Where(value => value.Col < col)
                    .OrderByDescending(value => value.Col)
                    .ThenBy(value => Math.Abs(value.Row - row))
                    .ToArray();

                newChoice = orderedRow.Concat(orderedRows).Concat(orderedColumns).FirstOrDefault();
            }

            if (allowEmptyRows) return newChoice;

            return newChoice != null && newChoice.Option.IsNotNullOrEmpty() ? newChoice : originalChoice;
        }

        public static GridMenuOption<T> GetInitialMatrixPosition<T>(this GridMenu<T> gridMenu, bool allowEmptyFields) {
            var rows = gridMenu.GetRowsFlattened();
            return allowEmptyFields ? rows.First() : rows.First(matrixValue => matrixValue.Option.IsNotNullOrEmpty());
        }
    }
}