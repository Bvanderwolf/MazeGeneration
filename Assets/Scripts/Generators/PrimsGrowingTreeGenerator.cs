using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class PrimsGrowingTreeGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the growing tree algorithm its prim version
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> bag = new List<MazeCell>();
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startPoint = service.StartCell;
            startPoint.MarkAsVisited();
            visited.Add(startPoint);
            bag.Add(startPoint);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick a random cell from the bag and check if it has unvisited neighbours
                MazeCell randomCell = bag[Random.Range(0, bag.Count)];
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(randomCell);
                if (unvisitedNeighbours.Count > 0)
                {
                    //pick a random unvisited neighbour and mark it as visited
                    MazeCell randomUnvisitedNeighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                    randomUnvisitedNeighbour.MarkAsVisited();
                    visited.Add(randomUnvisitedNeighbour);

                    //create passage between random cell and neighbour
                    randomCell.CreatePassage(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.CreatePassage(randomCell);

                    //add the random unvisited neighbour to the bag
                    bag.Add(randomUnvisitedNeighbour);
                }
                else
                {
                    //if the cell doesn't have any unvisited neighbours, remove it from the bag
                    bag.Remove(randomCell);
                }
            }
        }

        /// <summary>
        /// Returns an Enumerator that creates a maze by linking cells using the growing tree algorithm its prim version
        /// </summary>
        /// <param name="service"></param>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> bag = new List<MazeCell>();
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startPoint = service.StartCell;
            startPoint.MarkAsVisited();
            visited.Add(startPoint);
            bag.Add(startPoint);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick a random cell from the bag and check if it has unvisited neighbours
                MazeCell randomCell = bag[Random.Range(0, bag.Count)];
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(randomCell);
                if (unvisitedNeighbours.Count > 0)
                {
                    //pick a random unvisited neighbour and mark it as visited
                    MazeCell randomUnvisitedNeighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                    randomUnvisitedNeighbour.MarkAsVisited();
                    visited.Add(randomUnvisitedNeighbour);

                    //create passage between cell and neighbour
                    randomCell.CreatePassage(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.CreatePassage(randomCell);

                    //add the random unvisited neighbour to the bag
                    bag.Add(randomUnvisitedNeighbour);
                }
                else
                {
                    //if the cell doesn't have any unvisited neighbours, remove it from the bag
                    bag.Remove(randomCell);
                }

                randomCell.ShowAsStep(true);
                yield return null;
                randomCell.ShowAsStep(false);
            }

            CompletedRoutine();
        }
    }
}