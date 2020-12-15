using System;
using System.Collections;

namespace BWolf.MazeGeneration.Generators
{
    /// <summary>An abstract class for maze generators to derive from to be used by the maze generation service</summary>
    public abstract class MazeGenerator
    {
        public Action CompletedRoutine;

        public abstract void CreateMaze(MazeGenerationService service);

        public abstract IEnumerator CreateMazeRoutine(MazeGenerationService service);
    }
}