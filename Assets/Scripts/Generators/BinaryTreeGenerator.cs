using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class BinaryTreeGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the binary tree algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            foreach (MazeCell cell in service.Cells)
            {
                //mark cell as visited
                cell.MarkAsVisited();

                //retreive the neighbours
                List<MazeCell> neighbours = GetTopAndRightNeighbours(cell, service);
                if (neighbours.Count > 0)
                {
                    //pick a random neighbour
                    MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                    //create passage between cell and neighbour
                    randomNeighbour.CreatePassage(cell);
                    cell.CreatePassage(randomNeighbour);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using the binary tree algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            foreach (MazeCell cell in service.Cells)
            {
                //mark cell as visited
                cell.MarkAsVisited();

                //retreive the neighbours
                List<MazeCell> neighbours = GetTopAndRightNeighbours(cell, service);
                if (neighbours.Count > 0)
                {
                    //pick a random neighbour
                    MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                    //create passage between cell and neighbour
                    randomNeighbour.CreatePassage(cell);
                    cell.CreatePassage(randomNeighbour);
                }

                yield return null;
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Returns the top and right neighbours of given cell using the maze generation service
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private List<MazeCell> GetTopAndRightNeighbours(MazeCell cell, MazeGenerationService service)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = service.GetTopNeighbour(cell);
            if (top != null)
            {
                cells.Add(top);
            }

            MazeCell right = service.GetRightNeighbour(cell);
            if (right != null)
            {
                cells.Add(right);
            }

            return cells;
        }
    }
}