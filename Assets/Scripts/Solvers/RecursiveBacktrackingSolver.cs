using BWolf.MazeGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeSolving
{
    public class RecursiveBacktrackingSolver : MazeSolver
    {
        /// <summary>
        /// Solves the maze by creating a solution path using the Recursive Backtracking Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void SolveMaze(MazeSolvingService service)
        {
            //create datastructues necessary for algorithm to work
            Stack<MazeCell> stack = new Stack<MazeCell>();

            //set starting point and store exit for loop condition
            MazeCell exitCell = service.ExitCell;
            MazeCell entrance = service.EntranceCell;
            entrance.MarkAsPartOfSolution();
            stack.Push(entrance);

            //loop until the next cell in path is the exit
            MazeCell nextCellInPath = null;
            while (nextCellInPath != exitCell)
            {
                //look at the cell at the top of the stack and get the next cells in the path relative to it
                MazeCell cell = stack.Peek();
                List<MazeCell> cellsNextInPath = service.GetNextCellsInPath(cell);

                //check if the path is a dead end or not
                if (cellsNextInPath.Count > 0)
                {
                    //fetch a random cell that is next in the path and mark it as part of the solution
                    nextCellInPath = cellsNextInPath[Random.Range(0, cellsNextInPath.Count)];
                    nextCellInPath.MarkAsPartOfSolution();

                    //mark the walls as part of the solution aswell
                    cell.MarkWallAsPartOfSolution(nextCellInPath);
                    nextCellInPath.MarkWallAsPartOfSolution(cell);

                    //push the next cell to the stack
                    stack.Push(nextCellInPath);
                }
                else
                {
                    //mark the dead end and its broken walls as checked, so not part of the solution anymore
                    cell.MarkAsChecked();
                    cell.MarkBrokenWallsAsChecked();

                    //pop the stack to backtrack
                    stack.Pop();
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that solves the maze by creating a solution path using the Recursive Backtracking Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override IEnumerator SolveMazeRoutine(MazeSolvingService service)
        {
            //create datastructues necessary for algorithm to work
            Stack<MazeCell> stack = new Stack<MazeCell>();

            //set starting point and store exit for loop condition
            MazeCell exitCell = service.ExitCell;
            MazeCell entrance = service.EntranceCell;
            entrance.MarkAsPartOfSolution();
            stack.Push(entrance);

            //loop until the next cell in path is the exit
            MazeCell nextCellInPath = null;
            while (nextCellInPath != exitCell)
            {
                //look at the cell at the top of the stack and get the next cells in the path relative to it
                MazeCell cell = stack.Peek();
                List<MazeCell> cellsNextInPath = service.GetNextCellsInPath(cell);

                //check if the path is a dead end or not
                if (cellsNextInPath.Count > 0)
                {
                    //fetch a random cell that is next in the path and mark it as part of the solution
                    nextCellInPath = cellsNextInPath[Random.Range(0, cellsNextInPath.Count)];
                    nextCellInPath.MarkAsPartOfSolution();

                    //mark the walls as part of the solution aswell
                    cell.MarkWallAsPartOfSolution(nextCellInPath);
                    nextCellInPath.MarkWallAsPartOfSolution(cell);

                    //push the next cell to the stack
                    stack.Push(nextCellInPath);
                }
                else
                {
                    //mark the dead end and its broken walls as checked, so not part of the solution anymore
                    cell.MarkAsChecked();
                    cell.MarkBrokenWallsAsChecked();

                    //pop the stack to backtrack
                    stack.Pop();
                }

                yield return null;
            }

            CompletedRoutine();
        }
    }
}