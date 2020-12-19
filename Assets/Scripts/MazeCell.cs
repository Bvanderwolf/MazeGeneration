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
        private GameObject step = null;

        [SerializeField]
        private SpriteRenderer exitOrEntryIndicator = null;

        [Header("Walls")]
        [SerializeField]
        private SpriteRenderer topWall = null;

        [SerializeField]
        private SpriteRenderer bottomWall = null;

        [SerializeField]
        private SpriteRenderer leftWall = null;

        [SerializeField]
        private SpriteRenderer rightWall = null;

        private static readonly Color colorAsPassage = Color.white;
        private static readonly Color colorAsStep = Color.red;
        private static readonly Color colorAtAwake = Color.black;

        private SpriteRenderer spriteRenderer;

        public int MazeX { get; private set; }
        public int MazeY { get; private set; }

        public int SetNumber { get; private set; }

        public bool IsVisited { get; private set; }

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

            spriteRenderer.color = colorAtAwake;

            topWall.color = colorAtAwake;
            bottomWall.color = colorAtAwake;
            rightWall.color = colorAtAwake;
            leftWall.color = colorAtAwake;

            SetSetNumber(0);
            ShowSetNumber(false);

            ShowDirectionArrow(false);

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
            List<SpriteRenderer> edgewalls = GetEdgeWalls(solvingService.Cells.GetLength(0), solvingService.Cells.GetLength(1));
            edgewalls[Random.Range(0, edgewalls.Count)].color = colorAsPassage;

            //show entry on cell
            ShowAsExitOrEntry(true, solvingService.EntrySprite);
        }

        /// <summary>
        /// Mark this maze cell as exits, marking a wall as a passage towards outside of the maze
        /// </summary>
        public void MarkAsExit(MazeSolvingService solvingService)
        {
            //pick a random edge wall to destroy accounting for cases of cells being in a corner of the maze
            List<SpriteRenderer> edgewalls = GetEdgeWalls(solvingService.Cells.GetLength(0), solvingService.Cells.GetLength(1));
            edgewalls[Random.Range(0, edgewalls.Count)].color = colorAsPassage;

            //show exit on cell
            ShowAsExitOrEntry(true, solvingService.ExitSprite);
        }

        /// <summary>
        /// Returns a list of spriterenders of the walls of this cell that are on the edge of the maze
        /// </summary>
        /// <param name="gridWidth"></param>
        /// <param name="gridHeight"></param>
        /// <returns></returns>
        private List<SpriteRenderer> GetEdgeWalls(int gridWidth, int gridHeight)
        {
            List<SpriteRenderer> edgewalls = new List<SpriteRenderer>();

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

        /// <summary>
        /// Links this cell with given cell by breaking its wall relative to it
        /// </summary>
        /// <param name="cell"></param>
        public void Link(MazeCell cell)
        {
            if (cell.MazeY > MazeY)
            {
                //cell is top position relative to this cell
                topWall.color = colorAsPassage;
            }
            else if (cell.MazeY < MazeY)
            {
                //cell is bottom position relative to this cell
                bottomWall.color = colorAsPassage;
            }
            else if (cell.MazeX > MazeX)
            {
                //cell is right position relative to this cell
                rightWall.color = colorAsPassage;
            }
            else if (cell.MazeX < MazeX)
            {
                //cell is left position relative to this cell
                leftWall.color = colorAsPassage;
            }
            else
            {
                Debug.LogError($"Cell {name} is trying to link with itself :: this is not intended behaviour");
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
            step.SetActive(value);
        }

        /// <summary>
        /// Marks the the maze cell as visited
        /// </summary>
        public void MarkAsVisited()
        {
            spriteRenderer.color = colorAsPassage;
            IsVisited = true;
        }
    }
}