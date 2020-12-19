using BWolf.MazeGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BWolf.MazeSolving
{
    public class DeadEndFillingSolver : MazeSolver
    {
        /// <summary>
        /// Solves the maze by creating a solution path using the DeadEndFilling Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void SolveMaze(MazeSolvingService service)
        {
            //fill dead ends of the maze until junktions are found
            IEnumerable<MazeCell> deadEnds = service.Cells.Cast<MazeCell>().Where(cell => cell.IsDeadEnd);
            foreach (MazeCell deadEnd in deadEnds)
            {
                CheckUntilJunction(deadEnd, service);
            }

            //start walking from the entrance
            MazeCell walker = service.EntranceCell;
            walker.MarkAsPartOfSolution();

            //loop until the walker is at the exit
            MazeCell exitCell = service.ExitCell;
            while (walker != exitCell)
            {
                //get the next cell that is on the path towards the exit
                List<MazeCell> nextCellsInPath = service.GetNextCellsInPath(walker);
                if (nextCellsInPath.Count == 1)
                {
                    //make the cell part of the path solution
                    MazeCell nextCellInPath = nextCellsInPath[0];
                    nextCellInPath.MarkAsPartOfSolution();

                    //mark its walls as part of the solution aswell
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                    nextCellInPath.MarkWallAsPartOfSolution(walker);

                    //the next cell in the solution path is now the walker
                    walker = nextCellInPath;
                }
                else
                {
                    throw new System.InvalidOperationException($"The solution path could not be created from {walker.name} with {nextCellsInPath.Count} next cells in path :: this is unintended behaviour!");
                }
            }
        }

        /// <summary>
        /// Marks the maze as checked from given walker cell until a junction is met
        /// </summary>
        /// <param name="walker"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private void CheckUntilJunction(MazeCell walker, MazeSolvingService service)
        {
            walker.MarkAsChecked();

            //loop until a junction is found and is borken out of the loop
            while (true)
            {
                //get the next cell in the current path
                List<MazeCell> cellsNextInPath = service.GetNextCellsInPath(walker);
                if (cellsNextInPath.Count == 1)
                {
                    //save it is a junction
                    MazeCell nextCellInPath = cellsNextInPath[0];
                    bool isJunction = nextCellInPath.IsJunction;

                    //mark the walls as checked
                    walker.MarkWallAsChecked(nextCellInPath);
                    nextCellInPath.MarkWallAsChecked(walker);

                    if (isJunction)
                    {
                        //if the cell was a junction, break out of the loop
                        break;
                    }
                    else
                    {
                        //if the cell wasn't a junction, mark the cell as checked and continue from it
                        nextCellInPath.MarkAsChecked();
                        walker = nextCellInPath;
                    }
                }
                else
                {
                    throw new System.InvalidOperationException($"Filling in until junktion failed :: there should be one next cell in {walker}'s path each time");
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that solves the maze by creating a solution path using the DeadEndFilling Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override IEnumerator SolveMazeRoutine(MazeSolvingService service)
        {
            //fill dead ends of the maze until junktions are found
            IEnumerable<MazeCell> deadEnds = service.Cells.Cast<MazeCell>().Where(cell => cell.IsDeadEnd);
            foreach (MazeCell deadEnd in deadEnds)
            {
                yield return CheckUntilJunctionRoutine(deadEnd, service);
            }

            //start walking from the entrance
            MazeCell walker = service.EntranceCell;
            walker.MarkAsPartOfSolution();
            yield return null;

            //loop until the walker is at the exit
            MazeCell exitCell = service.ExitCell;
            while (walker != exitCell)
            {
                //get the next cell that is on current path
                List<MazeCell> nextCellsInPath = service.GetNextCellsInPath(walker);
                if (nextCellsInPath.Count == 1)
                {
                    //make the cell part of the path solution
                    MazeCell nextCellInPath = nextCellsInPath[0];
                    nextCellInPath.MarkAsPartOfSolution();

                    //mark its walls as part of the solution aswell
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                    nextCellInPath.MarkWallAsPartOfSolution(walker);

                    //the next cell in the path is now the walker
                    walker = nextCellInPath;
                }
                else
                {
                    throw new System.InvalidOperationException($"The solution path could not be created from {walker.name} with {nextCellsInPath.Count} next cells in path :: this is unintended behaviour!");
                }

                yield return null;
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Returns an enumerator that marks the maze as checked from given walker cell until a junction is met
        /// </summary>
        /// <param name="walker"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private IEnumerator CheckUntilJunctionRoutine(MazeCell walker, MazeSolvingService service)
        {
            walker.MarkAsChecked();
            yield return null;

            //loop until a junction is found and is borken out of the loop
            while (true)
            {
                //get the next cell in the current path
                List<MazeCell> cellsNextInPath = service.GetNextCellsInPath(walker);
                if (cellsNextInPath.Count == 1)
                {
                    //save it is a junction
                    MazeCell nextCellInPath = cellsNextInPath[0];
                    bool isJunction = nextCellInPath.IsJunction;

                    //mark the walls as checked
                    walker.MarkWallAsChecked(nextCellInPath);
                    nextCellInPath.MarkWallAsChecked(walker);

                    if (isJunction)
                    {
                        //if the cell was a junction, break out of the loop
                        break;
                    }
                    else
                    {
                        //if the cell wasn't a junction, mark the cell as checked and continue from it
                        nextCellInPath.MarkAsChecked();
                        walker = nextCellInPath;
                    }
                }
                else
                {
                    throw new System.InvalidOperationException($"Filling in until junktion failed :: there should be one next cell in {walker}'s path each time");
                }

                yield return null;
            }
        }
    }
}