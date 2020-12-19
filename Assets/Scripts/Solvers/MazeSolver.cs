using System;
using System.Collections;

namespace BWolf.MazeSolving
{
    public abstract class MazeSolver
    {
        public Action CompletedRoutine;

        public abstract void SolveMaze(MazeSolvingService service);

        public abstract IEnumerator SolveMazeRoutine(MazeSolvingService service);
    }
}