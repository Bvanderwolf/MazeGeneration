using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    /// <summary>
    /// Creates a maze by linking cells using prim's algorithm
    /// </summary>
    /// <param name="service"></param>
    public class PrimGenerator : MazeGenerator
    {
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            MazeCell[] cells = service.Cells.Cast<MazeCell>().ToArray();
            List<MazeCell> frontier = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();

            //add neighbours of starting cell to frontier
            frontier.AddRange(service.GetNeighbours(startCell));

            while (frontier.Count != 0)
            {
                //pick a random frontier cell, mark it as visited and remove it from the frontier
                MazeCell randomFrontierCell = frontier[Random.Range(0, frontier.Count)];
                randomFrontierCell.MarkAsVisited();
                frontier.Remove(randomFrontierCell);

                //pick a random visited neighbour of it and create passage between them
                List<MazeCell> visitedNeighbours = service.GetVisitedNeighbours(randomFrontierCell);
                if (visitedNeighbours.Count > 0)
                {
                    MazeCell randomVisitedNeighbour = visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
                    randomFrontierCell.CreatePassage(randomVisitedNeighbour);
                    randomVisitedNeighbour.CreatePassage(randomFrontierCell);
                }

                //add unvisited neighbours of it to the frontier if they aren't already in it
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(randomFrontierCell);
                foreach (MazeCell cell in unvisitedNeighbours)
                {
                    if (!frontier.Contains(cell))
                    {
                        frontier.Add(cell);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using prim's algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            MazeCell[] cells = service.Cells.Cast<MazeCell>().ToArray();
            List<MazeCell> frontier = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();

            //add neighbours of starting cell to frontier
            frontier.AddRange(service.GetNeighbours(startCell));

            while (frontier.Count != 0)
            {
                //pick a random frontier cell, mark it as visited and remove it from the frontier
                MazeCell randomFrontierCell = frontier[Random.Range(0, frontier.Count)];
                randomFrontierCell.MarkAsVisited();
                frontier.Remove(randomFrontierCell);

                //pick a random visited neighbour of it and link it with it
                List<MazeCell> visitedNeighbours = service.GetVisitedNeighbours(randomFrontierCell);
                if (visitedNeighbours.Count > 0)
                {
                    MazeCell randomVisitedNeighbour = visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
                    randomFrontierCell.CreatePassage(randomVisitedNeighbour);
                    randomVisitedNeighbour.CreatePassage(randomFrontierCell);
                }

                //add unvisited neighbours of it to the frontier if they aren't already in it
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(randomFrontierCell);
                foreach (MazeCell cell in unvisitedNeighbours)
                {
                    if (!frontier.Contains(cell))
                    {
                        frontier.Add(cell);
                    }
                }

                randomFrontierCell.ShowAsStep(true);
                yield return null;
                randomFrontierCell.ShowAsStep(false);
            }

            CompletedRoutine();
        }
    }
}