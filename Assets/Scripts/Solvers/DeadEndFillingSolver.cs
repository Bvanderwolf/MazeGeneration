using BWolf.MazeGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWolf.MazeSolving
{
    public class DeadEndFillingSolver : MazeSolver
    {
        public override void SolveMaze(MazeSolvingService service)
        {
        }

        public override IEnumerator SolveMazeRoutine(MazeSolvingService service)
        {
            IEnumerable<MazeCell> deadEnds = service.Cells.Cast<MazeCell>().Where(cell => cell.IsDeadEnd);
            foreach (MazeCell deadEnd in deadEnds)
            {
                yield return FillUntilJunktion(deadEnd, service);
            }

            CompletedRoutine();
        }

        private IEnumerator FillUntilJunktion(MazeCell walker, MazeSolvingService service)
        {
            walker.MarkAsChecked();
            yield return null;

            while (true)
            {
                List<MazeCell> cellsNextInPath = service.GetNextCellsInPath(walker);
                if (cellsNextInPath.Count == 1)
                {
                    MazeCell cell = cellsNextInPath[0];
                    bool isJunction = cell.IsJunction;

                    walker.MarkWallAsChecked(cell);
                    cell.MarkWallAsChecked(walker);

                    if (isJunction)
                    {
                        break;
                    }
                    else
                    {
                        walker = cell;
                        walker.MarkAsChecked();
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