﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class EllerGenerator : MazeGenerator
    {
        private bool debugMode;

        public EllerGenerator(bool debugMode)
        {
            this.debugMode = debugMode;
        }

        /// <summary>
        /// Creates a maze by linking cells using Eller's algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            MazeCell[,] gridcells = service.Cells;
            MazeCell[] cells = gridcells.Cast<MazeCell>().ToArray();

            //assign maze cells their set numbers
            AssignSetNumbers(gridcells);

            for (int y = 0; y < gridcells.GetLength(1); y++)
            {
                for (int x = 0; x < gridcells.GetLength(0); x++)
                {
                    //mark cell as visited
                    MazeCell cell = gridcells[x, y];
                    cell.MarkAsVisited();

                    List<MazeCell> neighbours = service.GetNeighboursNotPartOfSet(cell);
                    if (neighbours.Count > 0)
                    {
                        //pick a random neighbour not part of the cells set and link it with the cell
                        MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                        randomNeighbour.Link(cell);
                        cell.Link(randomNeighbour);

                        //all the cells belonging to the set the random neighbour belongs to are overtaken by the cell's set
                        MazeCell[] set = cells.Where(c => c.SetNumber == randomNeighbour.SetNumber).ToArray();
                        foreach (MazeCell overtakableCell in set)
                        {
                            overtakableCell.SetSetNumber(cell.SetNumber);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using Eller's algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            MazeCell[,] gridcells = service.Cells;
            MazeCell[] cells = gridcells.Cast<MazeCell>().ToArray();

            //assign maze cells their set numbers
            AssignSetNumbers(gridcells);

            for (int y = 0; y < gridcells.GetLength(1); y++)
            {
                for (int x = 0; x < gridcells.GetLength(0); x++)
                {
                    //mark cell as visited
                    MazeCell cell = gridcells[x, y];
                    cell.MarkAsVisited();

                    List<MazeCell> neighbours = service.GetNeighboursNotPartOfSet(cell);
                    if (neighbours.Count > 0)
                    {
                        //pick a random neighbour not part of the cells set and link it with the cell
                        MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                        randomNeighbour.Link(cell);
                        cell.Link(randomNeighbour);

                        //all the cells belonging to the set the random neighbour belongs to are overtaken by the cell's set
                        MazeCell[] set = cells.Where(c => c.SetNumber == randomNeighbour.SetNumber).ToArray();
                        foreach (MazeCell overtakableCell in set)
                        {
                            overtakableCell.SetSetNumber(cell.SetNumber);
                        }
                    }

                    yield return null;
                }
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Assigns the first rowgiven cells a set number based on their position
        /// </summary>
        /// <param name="cells"></param>
        private void AssignSetNumbers(MazeCell[,] cells)
        {
            int count = 0;
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    MazeCell cell = cells[x, y];
                    cell.SetSetNumber(count++);
                    cell.ShowSetNumber(debugMode);
                }
            }
        }
    }
}