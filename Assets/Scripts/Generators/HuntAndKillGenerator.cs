using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class HuntAndKillGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the HuntAndKill algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell walker = service.StartCell;
            walker.MarkAsVisited();
            visited.Add(walker);

            //loop untill all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //check if the current walker has unvisited neighbours
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(walker);
                if (unvisitedNeighbours.Count > 0)
                {
                    //pick a random unvisited neighbour and mark it as visited
                    MazeCell randomUnvisitedNeighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                    randomUnvisitedNeighbour.MarkAsVisited();
                    visited.Add(randomUnvisitedNeighbour);

                    //create passage between neighbour and walker
                    walker.CreatePassage(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.CreatePassage(walker);

                    //the random unvisited neighbour is now the walker
                    walker = randomUnvisitedNeighbour;
                }
                else
                {
                    //scan the grid for a hunted cell that is unvisited but has visited neighbours and mark it as visited
                    MazeCell huntedCell = GetRandomUnVisitedCellWithVisitedNeighbours(service);
                    huntedCell.MarkAsVisited();
                    visited.Add(huntedCell);

                    //fetch one of its visited neighbours and create passage between hunted cell and neighbour
                    List<MazeCell> visitedNeighbours = service.GetVisitedNeighbours(huntedCell);
                    MazeCell randomVisitedNeighbour = visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
                    huntedCell.CreatePassage(randomVisitedNeighbour);
                    randomVisitedNeighbour.CreatePassage(huntedCell);

                    //the hunted cell is now the walker
                    walker = huntedCell;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using the HuntAndKill algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell walker = service.StartCell;
            walker.MarkAsVisited();
            visited.Add(walker);

            //loop untill all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //check if the current walker has unvisited neighbours
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(walker);
                if (unvisitedNeighbours.Count > 0)
                {
                    //pick a random unvisited neighbour and mark it as visited
                    MazeCell randomUnvisitedNeighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                    randomUnvisitedNeighbour.MarkAsVisited();
                    visited.Add(randomUnvisitedNeighbour);

                    //create passage between cell and neighbour
                    walker.CreatePassage(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.CreatePassage(walker);

                    //the random unvisited neighbour is now the walker
                    walker = randomUnvisitedNeighbour;
                }
                else
                {
                    //scan the grid for a hunted cell that is unvisited but has visited neighbours and mark it as visited
                    MazeCell huntedCell = GetRandomUnVisitedCellWithVisitedNeighbours(service);
                    huntedCell.MarkAsVisited();
                    visited.Add(huntedCell);

                    //fetch one of its visited neighbours and link it with the hunted cell
                    List<MazeCell> visitedNeighbours = service.GetVisitedNeighbours(huntedCell);
                    MazeCell randomVisitedNeighbour = visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
                    huntedCell.CreatePassage(randomVisitedNeighbour);
                    randomVisitedNeighbour.CreatePassage(huntedCell);

                    //the hunted cell is now the walker
                    walker = huntedCell;
                }

                walker.ShowAsStep(true);
                yield return null;
                walker.ShowAsStep(false);
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Returns a random unvisited cell that has visited neighbours
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private MazeCell GetRandomUnVisitedCellWithVisitedNeighbours(MazeGenerationService service)
        {
            List<MazeCell> qualifiedCells = new List<MazeCell>();

            foreach (MazeCell cell in service.Cells)
            {
                if (!cell.IsVisited && service.GetVisitedNeighbours(cell).Count > 0)
                {
                    qualifiedCells.Add(cell);
                }
            }

            return qualifiedCells[Random.Range(0, qualifiedCells.Count)];
        }
    }
}