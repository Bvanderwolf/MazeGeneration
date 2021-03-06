﻿using UnityEngine;

namespace BWolf.MazeGeneration
{
    public class MazeCellWall : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public bool IsBroken { get; private set; }
        public bool IsChecked { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Breaks this wall, coloring it as a passage
        /// </summary>
        public void Break()
        {
            spriteRenderer.color = MazeCell.ColorAsPassage;
            IsBroken = true;
            IsChecked = false;
        }

        /// <summary>
        /// Marks the maze cell wall as checked
        /// </summary>
        public void MarkAsChecked()
        {
            spriteRenderer.color = MazeCell.ColorAsChecked;
            IsChecked = true;
        }

        /// <summary>
        /// Marks the maze cell wall as part of the solution path of the maze
        /// </summary>
        public void MarkAsPartOfSolution()
        {
            spriteRenderer.color = MazeCell.ColorAsSolution;
        }

        /// <summary>
        /// Sets the default values for this maze cell wall
        /// </summary>
        public void SetDefaultValues()
        {
            spriteRenderer.color = MazeCell.ColorAsWall;
            IsBroken = false;
            IsChecked = false;
        }
    }
}