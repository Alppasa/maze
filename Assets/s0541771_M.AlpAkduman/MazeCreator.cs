using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.s0541771_M.AlpAkduman
{
    public class MazeCreator
    {
        private readonly List<Cell> _cells;
        private int _currentColumn;

        private int _currentRow;
        private GameObject ExitTile;
        public bool MazeCompleted;
        protected int RowCount, ColumnCount, XHalfExt, ZHalfExt;


        public MazeCreator(List<Cell> cells, int xHalfExt, int zHalfExt, GameObject exitTile)
        {
            _cells = cells;
            var rowCount = 2 * xHalfExt + 1;
            var columnCount = 2 * zHalfExt + 1;
            RowCount = rowCount;
            ColumnCount = columnCount;
            XHalfExt = xHalfExt;
            ZHalfExt = zHalfExt;
            _currentRow = -xHalfExt;
            _currentColumn = -zHalfExt;
            ExitTile = exitTile;
        }

        public void CreateMaze()
        {
            var currentPosition = new Vector2(_currentRow, _currentColumn);
            var cell = _cells.FirstOrDefault(x => x.Position == currentPosition);
            //cell.Visited = true;

            while (!MazeCompleted)
            {
                DestroyWall(); // Will run until it hits a dead end.
                Search(); //Search for not visited Cells.
            }
            SetExitTile(GetCell(_currentRow, _currentColumn).Floor);
        }

        public Cell GetCell(int row, int column)
        {
            var currentPosition = new Vector2(row, column);
            var cell = _cells.FirstOrDefault(x => x.Position == currentPosition);
            return cell;
        }

        private void DestroyWall()
        {
            while (RouteStillAvailable(_currentRow, _currentColumn))
            {
                int direction = Random.Range(1, 5);
                //var direction = ProceduralNumberGenerator.GetNextNumber();

                if (direction == 1 && CellIsAvailable(_currentRow - 1, _currentColumn))
                {
                    // north
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn).NorthWall);
                    DestroyWallIfItExists(GetCell(_currentRow - 1, _currentColumn).SouthWall);
                    _currentRow--;
                }
                else if (direction == 2 && CellIsAvailable(_currentRow + 1, _currentColumn))
                {
                    // south
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn).SouthWall);
                    DestroyWallIfItExists(GetCell(_currentRow + 1, _currentColumn).NorthWall);
                    _currentRow++;
                }
                else if (direction == 3 && CellIsAvailable(_currentRow, _currentColumn + 1))
                {
                    // east
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn).EastWall);
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn + 1).WestWall);
                    _currentColumn++;
                }
                else if (direction == 4 && CellIsAvailable(_currentRow, _currentColumn - 1))
                {
                    // west
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn).WestWall);
                    DestroyWallIfItExists(GetCell(_currentRow, _currentColumn - 1).EastWall);
                    _currentColumn--;
                }

                GetCell(_currentRow, _currentColumn).Visited = true;
            }

        }

        private void Search()
        {
            MazeCompleted = true; // Set it to this, and see if we can prove otherwise below!

            for (var r = -XHalfExt; r <= XHalfExt; r++)
            {
                for (var c = -ZHalfExt; c <= ZHalfExt; c++)
                {
                    if (!GetCell(r, c).Visited && CellHasAnAdjacentVisitedCell(r, c))
                    {
                        MazeCompleted = false; // Yep, we found something so definitely do another DestroyWall cycle.
                        _currentRow = r;
                        _currentColumn = c;
                        DestroyAdjacentWall(_currentRow, _currentColumn);
                        GetCell(_currentRow, _currentColumn).Visited = true;
                        return;
                    }
                }
            }
        }


        private bool RouteStillAvailable(int row, int column)
        {
            var availableRoutes = 0;

            if (row > -XHalfExt && !GetCell(row - 1, column).Visited)
                availableRoutes++;

            if (row <= XHalfExt - 1 && !GetCell(row + 1, column).Visited)
                availableRoutes++;

            if (column > -ZHalfExt && !GetCell(row, column - 1).Visited)
                availableRoutes++;

            if (column <= ZHalfExt - 1 && !GetCell(row, column + 1).Visited)
                availableRoutes++;

            return availableRoutes > 0;
        }

        private bool CellIsAvailable(int row, int column)
        {
            if (row >= -XHalfExt && row <= XHalfExt && column >= -ZHalfExt && column <= ZHalfExt &&
                !GetCell(row, column).Visited)
                return true;
            return false;
        }

        private void DestroyWallIfItExists(GameObject wall)
        {
            if (wall != null) Object.Destroy(wall);
        }

        private void SetExitTile(GameObject floor)
        {
            if (floor != null)
            {
                Object.Destroy(floor);
                var tileSize = 4f;
                var root = GameObject.Find("MovablePlayfield");
                var cell = GetCell(_currentRow, _currentColumn);
                Object.Instantiate(ExitTile, new Vector3(_currentRow * tileSize, 3, _currentColumn * tileSize),
                Quaternion.identity, root.transform);
                cell.Floor = floor;
            }

        }

        private bool CellHasAnAdjacentVisitedCell(int row, int column)
        {
            var visitedCells = 0;

            // Look 1 row up (north) if we're on row 1 or greater
            if (row > -XHalfExt && GetCell(row - 1, column).Visited)
                visitedCells++;

            // Look one row down (south) if we're the second-to-last row (or less)
            if (row < XHalfExt - 2 && GetCell(row + 1, column).Visited)
                visitedCells++;

            // Look one row left (west) if we're column 1 or greater
            if (column > -ZHalfExt && GetCell(row, column - 1).Visited)
                visitedCells++;

            // Look one row right (east) if we're the second-to-last column (or less)
            if (column < ZHalfExt - 2 && GetCell(row, column + 1).Visited)
                visitedCells++;

            // return true if there are any adjacent visited cells to this one
            return visitedCells > 0;
        }

        private void DestroyAdjacentWall(int row, int column)
        {
            var wallDestroyed = false;

            while (!wallDestroyed)
            {
                //var direction = Random.Range(1, 5);
                var direction = ProceduralNumberGenerator.GetNextNumber();

                if (direction == 1 && row > -XHalfExt && GetCell(row - 1, column).Visited)
                {
                    DestroyWallIfItExists(GetCell(row, column).NorthWall);
                    DestroyWallIfItExists(GetCell(row - 1, column).SouthWall);
                    wallDestroyed = true;
                }
                else if (direction == 2 && row < XHalfExt - 2 && GetCell(row + 1, column).Visited)
                {
                    DestroyWallIfItExists(GetCell(row, column).SouthWall);
                    DestroyWallIfItExists(GetCell(row + 1, column).NorthWall);
                    wallDestroyed = true;
                }
                else if (direction == 3 && column > -ZHalfExt && GetCell(row, column - 1).Visited)
                {
                    DestroyWallIfItExists(GetCell(row, column).WestWall);
                    DestroyWallIfItExists(GetCell(row, column - 1).EastWall);
                    wallDestroyed = true;
                }
                else if (direction == 4 && column < ZHalfExt - 2 && GetCell(row, column + 1).Visited)
                {
                    DestroyWallIfItExists(GetCell(row, column).EastWall);
                    DestroyWallIfItExists(GetCell(row, column + 1).WestWall);
                    wallDestroyed = true;
                }
            }
        }
    }
}