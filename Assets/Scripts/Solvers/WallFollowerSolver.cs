using BWolf.MazeGeneration;
using System.Collections;
using UnityEngine;

namespace BWolf.MazeSolving
{
    public class WallFollowerSolver : MazeSolver
    {
        private const float ANGLE_LEFT = 90.0f;
        private const float ANGLE_RIGHT = -90.0f;

        /// <summary>
        /// Solves the maze by creating a solution path using the WallFollower Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void SolveMaze(MazeSolvingService service)
        {
            //set starting point
            MazeCell walker = service.EntranceCell;
            walker.MarkAsPartOfSolution();

            //store direction for manipulation
            Vector2Int direction = Vector2Int.left;

            //loop until exit has been reached by the walker
            MazeCell exitCell = service.ExitCell;
            while (walker != exitCell)
            {
                //try first walking left and then walking forward. Rotate right if both fail
                MazeCell nextCellInPath = null;
                if (service.GetNextCellInPathRelative(walker, GetValueRotatedLeft(direction), ref nextCellInPath))
                {
                    //if we can walk left, first rotate left
                    Rotate(ANGLE_LEFT, ref direction);

                    //make the next cell in the path and the passage towards it part of the solution
                    nextCellInPath.MarkAsPartOfSolution();
                    nextCellInPath.MarkWallAsPartOfSolution(walker);
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                }
                else if (service.GetNextCellInPathRelative(walker, direction, ref nextCellInPath))
                {
                    //make the next cell in the path and the passage towards it part of the solution
                    nextCellInPath.MarkAsPartOfSolution();
                    nextCellInPath.MarkWallAsPartOfSolution(walker);
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                }
                else
                {
                    //rotate right to look for a new way to walk left or forward
                    Rotate(ANGLE_RIGHT, ref direction);
                }

                if (nextCellInPath != null)
                {
                    //if the next cell in the path was found, we can walk on from it
                    walker = nextCellInPath;

                    if (service.DebugMode)
                    {
                        walker.SetDirectionArrow(direction);
                        walker.ShowDirectionArrow(true);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that solves the maze by creating a solution path using the WallFollower Algorithm
        /// </summary>
        /// <param name="service"></param>
        public override IEnumerator SolveMazeRoutine(MazeSolvingService service)
        {
            //set starting point
            MazeCell walker = service.EntranceCell;
            walker.MarkAsPartOfSolution();

            //store direction for manipulation
            Vector2Int direction = Vector2Int.left;

            //loop until exit has been reached by the walker
            MazeCell exitCell = service.ExitCell;
            while (walker != exitCell)
            {
                //try first walking left and then walking forward. Rotate right if both fail
                MazeCell nextCellInPath = null;
                if (service.GetNextCellInPathRelative(walker, GetValueRotatedLeft(direction), ref nextCellInPath))
                {
                    //if we can walk left, first rotate left
                    Rotate(ANGLE_LEFT, ref direction);

                    //make the next cell in the path and the passage towards it part of the solution
                    nextCellInPath.MarkAsPartOfSolution();
                    nextCellInPath.MarkWallAsPartOfSolution(walker);
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                }
                else if (service.GetNextCellInPathRelative(walker, direction, ref nextCellInPath))
                {
                    //make the next cell in the path and the passage towards it part of the solution
                    nextCellInPath.MarkAsPartOfSolution();
                    nextCellInPath.MarkWallAsPartOfSolution(walker);
                    walker.MarkWallAsPartOfSolution(nextCellInPath);
                }
                else
                {
                    //rotate right to look for a new way to walk left or forward
                    Rotate(ANGLE_RIGHT, ref direction);
                }

                if (nextCellInPath != null)
                {
                    //if the next cell in the path was found, we can walk on from it
                    walker = nextCellInPath;

                    if (service.DebugMode)
                    {
                        walker.SetDirectionArrow(direction);
                        walker.ShowDirectionArrow(true);
                    }
                }

                yield return null;
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Returns the given direction rotated to the left
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector2Int GetValueRotatedLeft(Vector2Int direction)
        {
            Vector2Int value = direction;

            Rotate(ANGLE_LEFT, ref value);

            return value;
        }

        /// <summary>
        /// Rotates the given direction by given degrees
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="direction"></param>
        private void Rotate(float degree, ref Vector2Int direction)
        {
            float rad = degree * Mathf.Deg2Rad;
            int xtemp = direction.x;
            direction.x = Mathf.RoundToInt(direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad));
            direction.y = Mathf.RoundToInt(xtemp * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad));
        }
    }
}