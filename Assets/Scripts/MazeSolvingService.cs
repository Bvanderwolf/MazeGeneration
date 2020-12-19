using BWolf.MazeGeneration;
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
        private Vector2Int exit = new Vector2Int(2, 0);

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

        private void Awake()
        {
            CreateSolvers();
        }

        private void CreateSolvers()
        {
            solvers.Add(Algorithm.DeadEndFilling, new DeadEndFillingSolver());
        }

        /// <summary>
        /// Starts the solving of the maze
        /// </summary>
        public void Solve()
        {
            if (IsSolving || generationService.IsGenerating)
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

            StartCoroutine(solvers[algorithm].SolveMazeRoutine(this));
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

        private enum Algorithm
        {
            DeadEndFilling
        }
    }
}