using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class AldousBroderGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the aldous broder algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();
            visited.Add(startCell);

            //set current cel as starting cell
            MazeCell currentCell = startCell;

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick a random neighbour of the current cell
                List<MazeCell> neighbours = service.GetNeighbours(currentCell);
                MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                if (!randomNeighbour.IsVisited)
                {
                    //if the random neighbour hasn't been visited yet, mark it as visited
                    randomNeighbour.MarkAsVisited();
                    visited.Add(randomNeighbour);

                    //create passage between current cell and neighbour
                    randomNeighbour.CreatePassage(currentCell);
                    currentCell.CreatePassage(randomNeighbour);
                }

                //set current cell to random neighbour to travel from it
                currentCell = randomNeighbour;
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using the adlous broder algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();
            visited.Add(startCell);

            //set current cel as starting cell
            MazeCell currentCell = startCell;

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick a random neighbour of the current cell
                List<MazeCell> neighbours = service.GetNeighbours(currentCell);
                MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                if (!randomNeighbour.IsVisited)
                {
                    //if the random neighbour hasn't been visited yet, mark it as visited
                    randomNeighbour.MarkAsVisited();
                    visited.Add(randomNeighbour);

                    //create passage between cell and neighbour
                    randomNeighbour.CreatePassage(currentCell);
                    currentCell.CreatePassage(randomNeighbour);
                }

                //set current cell to random neighbour to travel from it
                currentCell = randomNeighbour;

                //show stepping feedback
                currentCell.ShowAsStep(true);
                yield return null;
                currentCell.ShowAsStep(false);
            }

            CompletedRoutine();
        }
    }
}