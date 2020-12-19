using BWolf.MazeGeneration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeSolving
{
    public class MazeSolvingService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Algorithm algorithm = Algorithm.DeadEndFilling;

        [SerializeField]
        private Vector2Int entry = Vector2Int.zero;

        [SerializeField]
        private Vector2Int exit = Vector2Int.zero;

        [Space]
        [SerializeField]
        private bool slowMode = true;

        [Header("Scene References")]
        [SerializeField]
        private MazeGenerationService generationService = null;

        [Header("Project References")]
        [SerializeField]
        private Sprite entrySprite = null;

        [SerializeField]
        private Sprite exitSprite = null;

        private Dictionary<Algorithm, MazeSolver> solvers = new Dictionary<Algorithm, MazeSolver>();

        public MazeCell[,] Cells
        {
            get
            {
                return generationService.Cells;
            }
        }

        public Sprite EntrySprite
        {
            get
            {
                return entrySprite;
            }
        }

        public Sprite ExitSprite
        {
            get
            {
                return exitSprite;
            }
        }

        public bool IsSolving { get; private set; }
        public bool HasSolvedMaze { get; private set; }

        private void Awake()
        {
            CreateSolvers();
        }

        private void CreateSolvers()
        {
            solvers.Add(Algorithm.DeadEndFilling, new DeadEndFillingSolver());

            foreach (MazeSolver generator in solvers.Values)
            {
                generator.CompletedRoutine += OnSolvingRoutineCompleted;
            }
        }

        private void OnSolvingRoutineCompleted()
        {
            IsSolving = false;
            HasSolvedMaze = true;
        }

        /// <summary>
        /// Starts the solving of the maze
        /// </summary>
        public void Solve()
        {
            if (IsSolving || HasSolvedMaze || generationService.IsGenerating)
            {
                return;
            }

            //reset all entries and exits currently in the maze
            ResetEntriesAndExits();

            //try creating a new entry and a new exit
            if (!CreateEntry() || !CreateExit())
            {
                return;
            }

            if (slowMode)
            {
                IsSolving = true;
                StartCoroutine(solvers[algorithm].SolveMazeRoutine(this));
            }
            else
            {
                solvers[algorithm].SolveMaze(this);
                HasSolvedMaze = true;
            }
        }

        /// <summary>
        /// Reset all entries and exits currently in the maze
        /// </summary>
        private void ResetEntriesAndExits()
        {
            foreach (MazeCell cell in generationService.Cells)
            {
                cell.ShowAsExitOrEntry(false, null);
            }
        }

        /// <summary>
        /// Tries creating an entry based on stored "entry" value. Returns whether it succeeded
        /// </summary>
        /// <returns></returns>
        private bool CreateEntry()
        {
            MazeCell[,] cells = generationService.Cells;
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            if ((entry.x != 0 && entry.x != width - 1) && (entry.y != 0 && entry.y != height - 1))
            {
                Debug.LogError("Failed to create entry :: entries can only be created at the edges of the maze");
                return false;
            }
            else
            {
                cells[entry.x, entry.y].MarkAsEntry(this);
                return true;
            }
        }

        /// <summary>
        /// Tries creating an exit based on stored "exit" value. Returns whether it succeeded
        /// </summary>
        /// <returns></returns>
        private bool CreateExit()
        {
            if (exit == entry)
            {
                Debug.LogError("Failed to create exit :: exit can not be at the same position as the entry");
                return false;
            }

            MazeCell[,] cells = generationService.Cells;
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            if ((exit.x != 0 && exit.x != width - 1) && (exit.y != 0 && exit.y != height - 1))
            {
                Debug.LogError("Failed to create exit :: exits can only be created at the edges of the maze");
                return false;
            }
            else
            {
                cells[exit.x, exit.y].MarkAsExit(this);
                return true;
            }
        }

        public List<MazeCell> GetNextCellsInPath(MazeCell cell)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = generationService.GetTopNeighbour(cell);
            if (top != null && !top.IsChecked && cell.HasPassageTowardsCell(top))
            {
                cells.Add(top);
            }

            MazeCell bottom = generationService.GetBottomNeighbour(cell);
            if (bottom != null && !bottom.IsChecked && cell.HasPassageTowardsCell(bottom))
            {
                cells.Add(bottom);
            }

            MazeCell left = generationService.GetLeftNeighbour(cell);
            if (left != null && !left.IsChecked && cell.HasPassageTowardsCell(left))
            {
                cells.Add(left);
            }

            MazeCell right = generationService.GetRightNeighbour(cell);
            if (right != null && !right.IsChecked && cell.HasPassageTowardsCell(right))
            {
                cells.Add(right);
            }

            return cells;
        }

        private enum Algorithm
        {
            DeadEndFilling
        }
    }
}