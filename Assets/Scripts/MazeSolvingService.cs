﻿using BWolf.MazeGeneration;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BWolf.MazeSolving
{
    public class MazeSolvingService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Algorithm algorithm = Algorithm.DeadEndFilling;

        [SerializeField, Tooltip("The position on the grid where the solver will start its solution path")]
        private Vector2Int entrance = Vector2Int.zero;

        [SerializeField, Tooltip("The position on the grid where the solver will end its solution path")]
        private Vector2Int exit = Vector2Int.zero;

        [Space]
        [SerializeField, Tooltip("If slow mode is set to true, the solver will wait a video frame each step resulting in a slower process")]
        private bool slowMode = true;

        [Header("Scene References")]
        [SerializeField]
        private Toggle toggleSlowMode = null;

        [Space]
        [SerializeField]
        private Text textAlgorithm = null;

        [Space]
        [SerializeField]
        private MazeGenerationService generationService = null;

        [Header("Project References")]
        [SerializeField]
        private Sprite entranceSprite = null;

        [SerializeField]
        private Sprite exitSprite = null;

        private Dictionary<Algorithm, MazeSolver> solvers = new Dictionary<Algorithm, MazeSolver>();

        /// <summary>
        /// A reference to the 2D grid of cells generated by the maze generation service's generator
        /// </summary>
        public MazeCell[,] Cells
        {
            get
            {
                return generationService.Cells;
            }
        }

        /// <summary>
        /// The sprite used for showing the entrance cell on screen
        /// </summary>
        public Sprite EntranceSprite
        {
            get
            {
                return entranceSprite;
            }
        }

        /// <summary>
        /// The sprite used for showing the exit cell on screen
        /// </summary>
        public Sprite ExitSprite
        {
            get
            {
                return exitSprite;
            }
        }

        /// <summary>
        /// The entrance cell in the maze based on choosen "entrance" value
        /// </summary>
        public MazeCell EntranceCell
        {
            get
            {
                return generationService.Cells[entrance.x, entrance.y];
            }
        }

        /// <summary>
        /// The exit cell in the maze based on choosen "exit" value
        /// </summary>
        public MazeCell ExitCell
        {
            get
            {
                return generationService.Cells[exit.x, exit.y];
            }
        }

        public bool IsSolving { get; private set; }
        public bool HasSolvedMaze { get; private set; }

        private void Awake()
        {
            InitializeUserInterface();
            CreateSolvers();

            generationService.OnGeneratedMaze += OnMazeGenerated;
        }

        private void OnValidate()
        {
            UpdateAlgorithmText();
            UpdateToggles();
        }

        private void OnDestroy()
        {
            generationService.OnGeneratedMaze -= OnMazeGenerated;

            toggleSlowMode.onValueChanged.RemoveListener(OnSlowModeToggled);
        }

        /// <summary>
        /// Creates the MazeSolver instances for each algorithm
        /// </summary>
        private void CreateSolvers()
        {
            solvers.Add(Algorithm.DeadEndFilling, new DeadEndFillingSolver());
            solvers.Add(Algorithm.RecursiveBacktracking, new RecursiveBacktrackingSolver());

            foreach (MazeSolver generator in solvers.Values)
            {
                generator.CompletedRoutine += OnSolvingRoutineCompleted;
            }
        }

        private void InitializeUserInterface()
        {
            toggleSlowMode.onValueChanged.AddListener(OnSlowModeToggled);
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

            //start the solving process based on choosen algorithm in slowMode or instant
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

            if ((entrance.x != 0 && entrance.x != width - 1) && (entrance.y != 0 && entrance.y != height - 1))
            {
                Debug.LogError("Failed to create entry :: entries can only be created at the edges of the maze");
                return false;
            }
            else
            {
                cells[entrance.x, entrance.y].MarkAsEntry(this);
                return true;
            }
        }

        /// <summary>
        /// Tries creating an exit based on stored "exit" value. Returns whether it succeeded
        /// </summary>
        /// <returns></returns>
        private bool CreateExit()
        {
            if (exit == entrance)
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

        /// <summary>
        /// Resets the IsSolving flag and makes sure the same maze can't be re-solved again
        /// </summary>
        private void OnSolvingRoutineCompleted()
        {
            IsSolving = false;
            HasSolvedMaze = true;
        }

        /// <summary>
        /// Makes sure the maze generated can be solved
        /// </summary>
        private void OnMazeGenerated()
        {
            HasSolvedMaze = false;
        }

        /// <summary>
        /// Updates the slow mode function
        /// </summary>
        private void OnSlowModeToggled(bool value)
        {
            slowMode = value;
        }

        /// <summary>
        /// Updates the shown toggle states in the User Interface
        /// </summary>
        private void UpdateToggles()
        {
            toggleSlowMode.isOn = slowMode;
        }

        /// <summary>
        /// Updates the shown algorithm text in the User Interface
        /// </summary>
        private void UpdateAlgorithmText()
        {
            textAlgorithm.text = algorithm.ToString();
        }

        /// <summary>
        /// Cycles through the algorithm values to set the algorithm used for generation either to the next or previous one based on given "forward" value
        /// </summary>
        /// <param name="forward"></param>
        public void CycleThroughAlgorithmValues(bool forward)
        {
            if (forward)
            {
                int next = (int)algorithm + 1;
                if (next == System.Enum.GetValues(typeof(Algorithm)).Length)
                {
                    next = 0;
                }

                algorithm = (Algorithm)next;
            }
            else
            {
                int previous = (int)algorithm - 1;
                if (previous < 0)
                {
                    previous = System.Enum.GetValues(typeof(Algorithm)).Length - 1;
                }

                algorithm = (Algorithm)previous;
            }

            UpdateAlgorithmText();
        }

        /// <summary>
        /// Returns the cells that are adjecent to given cell, are not checked yet and are not blocked by a wall
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
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

        /// <summary>
        /// The algorithms used for solving the generated mazes
        /// </summary>
        private enum Algorithm
        {
            DeadEndFilling,
            RecursiveBacktracking
        }
    }
}