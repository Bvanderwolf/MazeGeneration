using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class RecursiveBacktrackingGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the recursive backtracking algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            Stack<MazeCell> stack = new Stack<MazeCell>();
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();
            visited.Add(startCell);
            stack.Push(startCell);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick the cell to branch from, from the top of the stack and retreive its neighbours
                MazeCell newest = stack.Peek();
                List<MazeCell> unvisitedNeighbours = service.GetUnVisitedNeighbours(newest);

                if (unvisitedNeighbours.Count > 0)
                {
                    //pick a random neighbour from the list
                    MazeCell neighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];

                    //set it as visited and add it to the visited list
                    neighbour.MarkAsVisited();
                    visited.Add(neighbour);

                    //create passage between newest and neighbour
                    newest.CreatePassage(neighbour);
                    neighbour.CreatePassage(newest);

                    //push the neighbour to the stack
                    stack.Push(neighbour);
                }
                else
                {
                    //if no unvisited neighbours are available, backtrack by popping the stack
                    stack.Pop();
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using the recursive backtracking algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            Stack<MazeCell> stack = new Stack<MazeCell>();
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startCell = service.StartCell;
            startCell.MarkAsVisited();
            visited.Add(startCell);
            stack.Push(startCell);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                //pick the cell to branch from, from the top of the stack and retreive its neighbours
                MazeCell newest = stack.Peek();
                List<MazeCell> neighbours = service.GetUnVisitedNeighbours(newest);

                if (neighbours.Count > 0)
                {
                    //pick a random neighbour from the list
                    MazeCell neighbour = neighbours[Random.Range(0, neighbours.Count)];

                    //set it as visited and add it to the visited list
                    neighbour.MarkAsVisited();
                    visited.Add(neighbour);

                    //create passage between newest and neighbour
                    newest.CreatePassage(neighbour);
                    neighbour.CreatePassage(newest);

                    //push the neighbour to the stack
                    stack.Push(neighbour);
                }
                else
                {
                    //if no unvisited neighbours are available, backtrack by popping the stack
                    stack.Pop();
                }

                newest.ShowAsStep(true);
                yield return null;
                newest.ShowAsStep(false);
            }

            CompletedRoutine();
        }
    }
}