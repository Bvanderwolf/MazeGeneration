using BWolf.MazeSolving;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration
{
    public class MazeCell : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField]
        private TextMesh setNumberText = null;

        [SerializeField]
        private Transform directionArrow = null;

        [SerializeField]
        private GameObject stepIndicator = null;

        [SerializeField]
        private SpriteRenderer exitOrEntryIndicator = null;

        [Header("Walls")]
        [SerializeField]
        private MazeCellWall topWall = null;

        [SerializeField]
        private MazeCellWall bottomWall = null;

        [SerializeField]
        private MazeCellWall leftWall = null;

        [SerializeField]
        private MazeCellWall rightWall = null;

        public static readonly Color ColorAsPassage = Color.white;
        public static readonly Color ColorAsWall = Color.black;
        public static readonly Color ColorAsChecked = Color.grey;
        public static readonly Color ColorAsSolution = Color.green;

        private static readonly Color colorAsStep = Color.red;

        private SpriteRenderer spriteRenderer;

        public int MazeX { get; private set; }
        public int MazeY { get; private set; }

        public int SetNumber { get; private set; }

        public bool IsVisited { get; private set; }
        public bool IsChecked { get; private set; }

        private const int DEAD_END_WALL_COUNT = 3;

        /// <summary>
        /// The amount of walls of this cell that are not broken
        /// </summary>
        private int brokenWalls
        {
            get
            {
                int count = 0;

                if (topWall.IsBroken) count++;
                if (bottomWall.IsBroken) count++;
                if (rightWall.IsBroken) count++;
                if (leftWall.IsBroken) count++;

                return count;
            }
        }

        private int checkedWalls
        {
            get
            {
                int count = 0;

                if (topWall.IsChecked) count++;
                if (bottomWall.IsChecked) count++;
                if (rightWall.IsChecked) count++;
                if (leftWall.IsChecked) count++;

                return count;
            }
        }

        /// <summary>
        /// Is this cell a dead end
        /// </summary>
        public bool IsDeadEnd
        {
            get
            {
                return brokenWalls == 1;
            }
        }

        /// <summary>
        /// Is this cell a junction
        /// </summary>
        public bool IsJunction
        {
            get
            {
                return brokenWalls - checkedWalls >= 3;
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Sets the default values for this maze cell
        /// </summary>
        public void SetDefaultValues()
        {
            IsVisited = false;
            IsChecked = false;

            spriteRenderer.color = ColorAsWall;

            topWall.SetDefaultValues();
            bottomWall.SetDefaultValues();
            rightWall.SetDefaultValues();
            leftWall.SetDefaultValues();

            SetSetNumber(0);
            ShowSetNumber(false);
            ShowDirectionArrow(false);
            ShowAsStep(false);
            ShowAsExitOrEntry(false, null);
        }

        /// <summary>
        /// Shows or hides cell as exit or entry based on given values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sprite"></param>
        public void ShowAsExitOrEntry(bool value, Sprite sprite)
        {
            exitOrEntryIndicator.gameObject.SetActive(value);
            exitOrEntryIndicator.sprite = sprite;
        }

        /// <summary>
        /// Mark this maze cell as entry, marking a wall as a passage towards outside of the maze
        /// </summary>
        public void MarkAsEntry(MazeSolvingService solvingService)
        {
            //pick a random edge wall to destroy accounting for cases of cells being in a corner of the maze
            List<MazeCellWall> edgewalls = GetEdgeWalls(solvingService.Cells.GetLength(0), solvingService.Cells.GetLength(1));
            MazeCellWall wall = edgewalls[Random.Range(0, edgewalls.Count)];

            //break the wall and mark it as part of the solution
            wall.Break();
            wall.MarkAsPartOfSolution();

            //show entry on cell
            ShowAsExitOrEntry(true, solvingService.EntranceSprite);
        }

        /// <summary>
        /// Mark this maze cell as exits, marking a wall as a passage towards outside of the maze
        /// </summary>
        public void MarkAsExit(MazeSolvingService solvingService)
        {
            //pick a random edge wall to destroy accounting for cases of cells being in a corner of the maze
            List<MazeCellWall> edgewalls = GetEdgeWalls(solvingService.Cells.GetLength(0), solvingService.Cells.GetLength(1));
            MazeCellWall wall = edgewalls[Random.Range(0, edgewalls.Count)];

            //break the wall and mark it as part of the solution
            wall.Break();
            wall.MarkAsPartOfSolution();

            //show exit on cell
            ShowAsExitOrEntry(true, solvingService.ExitSprite);
        }

        /// <summary>
        /// Returns a list of spriterenders of the walls of this cell that are on the edge of the maze
        /// </summary>
        /// <param name="gridWidth"></param>
        /// <param name="gridHeight"></param>
        /// <returns></returns>
        private List<MazeCellWall> GetEdgeWalls(int gridWidth, int gridHeight)
        {
            List<MazeCellWall> edgewalls = new List<MazeCellWall>();

            if (MazeX == 0)
            {
                //cell is on the left edge of the maze
                edgewalls.Add(leftWall);
            }
            else if (MazeX == gridWidth - 1)
            {
                //cell is on the right edge of the maze
                edgewalls.Add(rightWall);
            }

            if (MazeY == 0)
            {
                //cell is on the top edge of the maze
                edgewalls.Add(bottomWall);
            }
            else if (MazeY == gridHeight - 1)
            {
                //cell is on the bottom edge of the maze
                edgewalls.Add(topWall);
            }

            return edgewalls;
        }

        public bool HasPassageTowardsCell(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                return topWall.IsBroken;
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                return bottomWall.IsBroken;
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                return rightWall.IsBroken;
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                return leftWall.IsBroken;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a passage with given cell by breaking its wall relative to it
        /// </summary>
        /// <param name="cell"></param>
        public void CreatePassage(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                topWall.Break();
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                bottomWall.Break();
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                rightWall.Break();
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                leftWall.Break();
            }
            else
            {
                Debug.LogError($"Cell {name} is trying to link with itself :: this is not intended behaviour");
            }
        }

        /// <summary>
        /// Marks a wall as checked based on given cell relative to it
        /// </summary>
        /// <param name="cell"></param>
        public void MarkWallAsChecked(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                topWall.MarkAsChecked();
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                bottomWall.MarkAsChecked();
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                rightWall.MarkAsChecked();
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                leftWall.MarkAsChecked();
            }
            else
            {
                Debug.LogError($"Cell {name} is trying to mark a wall as checked based on itself :: this is not intended behaviour");
            }
        }

        /// <summary>
        /// Marks a wall as part of the solution path of the maze based on given cell relative to it
        /// </summary>
        /// <param name="cell"></param>
        public void MarkWallAsPartOfSolution(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                topWall.MarkAsPartOfSolution();
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                bottomWall.MarkAsPartOfSolution();
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                rightWall.MarkAsPartOfSolution();
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                leftWall.MarkAsPartOfSolution();
            }
            else
            {
                Debug.LogError($"Cell {name} is trying to mark a wall as checked based on itself :: this is not intended behaviour");
            }
        }

        /// <summary>
        /// Sets the direction arrow rotation based on the relative position of given cell to this one
        /// </summary>
        /// <param name="cell"></param>
        public void SetDirectionArrow(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                directionArrow.localEulerAngles = Vector3.zero;
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                directionArrow.localEulerAngles = new Vector3(0, 0, 180);
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                directionArrow.localEulerAngles = new Vector3(0, 0, -90);
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                directionArrow.localEulerAngles = new Vector3(0, 0, 90);
            }
        }

        /// <summary>
        /// Either shows or hides direction arrow based on given value
        /// </summary>
        /// <param name="value"></param>
        public void ShowDirectionArrow(bool value)
        {
            directionArrow.gameObject.SetActive(value);
        }

        /// <summary>
        /// Returns the relative direction to given cell from this cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public Vector2Int GetDirectionRelative(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                return Vector2Int.up;
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                return Vector2Int.down;
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                return Vector2Int.right;
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                return Vector2Int.left;
            }
            else
            {
                return Vector2Int.zero;
            }
        }

        /// <summary>
        /// Sets the maze cells x and y grid coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetMazeCoordinates(int x, int y)
        {
            MazeX = x;
            MazeY = y;
        }

        /// <summary>
        /// Sets the SetNumber value for this maze cell used in Kruskal's and Eller's Algorithm
        /// </summary>
        /// <param name="number"></param>
        public void SetSetNumber(int number)
        {
            SetNumber = number;
            setNumberText.text = SetNumber.ToString();
        }

        /// <summary>
        /// Either shows or hides set number text based on given value
        /// </summary>
        /// <param name="value"></param>
        public void ShowSetNumber(bool value)
        {
            setNumberText.gameObject.SetActive(value);
        }

        /// <summary>
        /// Colors the maze cell to be colored as step or not based on given value
        /// </summary>
        /// <param name="value"></param>
        public void ShowAsStep(bool value)
        {
            stepIndicator.SetActive(value);
        }

        /// <summary>
        /// Marks the the maze cell as visited
        /// </summary>
        public void MarkAsVisited()
        {
            spriteRenderer.color = ColorAsPassage;
            IsVisited = true;
        }

        /// <summary>
        /// Marks the maze cell as checked
        /// </summary>
        public void MarkAsChecked()
        {
            spriteRenderer.color = ColorAsChecked;
            IsChecked = true;
        }

        /// <summary>
        /// Marks the maze cell as part of the solution path of the maze
        /// </summary>
        public void MarkAsPartOfSolution()
        {
            spriteRenderer.color = ColorAsSolution;
            IsChecked = true;
        }
    }
}