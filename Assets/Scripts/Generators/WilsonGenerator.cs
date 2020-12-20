using BWolf.MazeGeneration.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class WilsonGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using Wilson's algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startPoint = service.RootCell;
            startPoint.MarkAsVisited();
            visited.Add(startPoint);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                RandomWalk(GetRandomUnvisitedCell(service), service, visited);
            }
        }

        /// <summary>
        /// Executes a random walk from given unvisited cell adding visited cells to visited list on finish
        /// </summary>
        /// <param name="unvisitedCell"></param>
        /// <param name="service"></param>
        /// <param name="visited"></param>
        private void RandomWalk(MazeCell unvisitedCell, MazeGenerationService service, List<MazeCell> visited)
        {
            //setup data structures necessary for the random walk to take place
            List<KeyValuePair<MazeCell, Vector2Int>> records = new List<KeyValuePair<MazeCell, Vector2Int>>();
            List<MazeCell> neighbours = service.GetNeighbours(unvisitedCell);

            //pick a random neighbour as the walking position and check whether it is not already visited
            MazeCell walker = neighbours[Random.Range(0, neighbours.Count)];
            if (walker.IsVisited)
            {
                //mark the unvisited cell as visited
                unvisitedCell.MarkAsVisited();
                visited.Add(unvisitedCell);

                //link unvisited cell to the walker and return
                unvisitedCell.CreatePassage(walker);
                walker.CreatePassage(unvisitedCell);
                return;
            }

            //get the direction relative to the walker so we can store it as a record
            Vector2Int direction = unvisitedCell.GetDirectionRelative(walker);
            records.Add(new KeyValuePair<MazeCell, Vector2Int>(unvisitedCell, direction));

            //loop until there are not records left
            while (records.Count != 0)
            {
                //refresh the neighbours list with the walkers neighbours
                neighbours.Clear();
                neighbours.AddRange(service.GetNeighbours(walker));

                //pick a random neighbour, and record the direction from the walker to it
                MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                direction = walker.GetDirectionRelative(randomNeighbour);

                if (!records.TryUpdate(walker, direction))
                {
                    records.Add(new KeyValuePair<MazeCell, Vector2Int>(walker, direction));
                }

                if (randomNeighbour.IsVisited)
                {
                    //if the random neighbour is visited, carve out a path using the records and clear the records list
                    MarkRecordAsVisitedRecursive(records[0], records, service, visited);
                    records.Clear();
                }
                else
                {
                    //if the random neighbour isn't visited yet, set it as the new walker cell
                    walker = randomNeighbour;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using Wilson's algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //create data structures necessary for algorithm to work
            List<MazeCell> visited = new List<MazeCell>();

            //set starting point
            MazeCell startPoint = service.RootCell;
            startPoint.MarkAsVisited();
            visited.Add(startPoint);

            //loop until all cells have been visited
            long totalCells = service.Cells.LongLength;
            while (visited.Count != totalCells)
            {
                yield return RandomWalkRoutine(GetRandomUnvisitedCell(service), service, visited);
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Returns a routine that executes a random walk from given unvisited cell adding visited cells to visited list on finish
        /// </summary>
        /// <param name="unvisitedCell"></param>
        /// <param name="service"></param>
        /// <param name="visited"></param>
        private IEnumerator RandomWalkRoutine(MazeCell unvisitedCell, MazeGenerationService service, List<MazeCell> visited)
        {
            //setup data structures necessary for the random walk to take place
            List<KeyValuePair<MazeCell, Vector2Int>> records = new List<KeyValuePair<MazeCell, Vector2Int>>();
            List<MazeCell> neighbours = service.GetNeighbours(unvisitedCell);

            //pick a random neighbour as the walking position and check whether it is not already visited
            MazeCell walker = neighbours[Random.Range(0, neighbours.Count)];
            if (walker.IsVisited)
            {
                //mark the unvisited cell as visited
                unvisitedCell.MarkAsVisited();
                visited.Add(unvisitedCell);

                //link unvisited cell to the walker and break out of the routine
                unvisitedCell.CreatePassage(walker);
                walker.CreatePassage(unvisitedCell);
                yield break;
            }

            //get the direction relative to the walker so we can store it as a record
            Vector2Int direction = unvisitedCell.GetDirectionRelative(walker);
            if (service.DebugMode)
            {
                unvisitedCell.SetDirectionArrow(walker);
                unvisitedCell.ShowDirectionArrow(true);
            }
            records.Add(new KeyValuePair<MazeCell, Vector2Int>(unvisitedCell, direction));

            //loop until there are not records left
            while (records.Count != 0)
            {
                //refresh the neighbours list with the walkers neighbours
                neighbours.Clear();
                neighbours.AddRange(service.GetNeighbours(walker));

                //pick a random neighbour, and record the direction from the walker to it
                MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                direction = walker.GetDirectionRelative(randomNeighbour);
                if (service.DebugMode)
                {
                    walker.SetDirectionArrow(randomNeighbour);
                    walker.ShowDirectionArrow(true);
                }

                if (!records.TryUpdate(walker, direction))
                {
                    records.Add(new KeyValuePair<MazeCell, Vector2Int>(walker, direction));
                }

                if (randomNeighbour.IsVisited)
                {
                    //if the random neighbour is visited, carve out a path using the records and clear the records list
                    MarkRecordAsVisitedRecursive(records[0], records, service, visited);
                    if (service.DebugMode)
                    {
                        records.ForEach(record => record.Key.ShowDirectionArrow(false));
                    }
                    records.Clear();
                }
                else
                {
                    //if the random neighbour isn't visited yet, set it as the new walker cell
                    walker = randomNeighbour;
                }

                walker.ShowAsStep(true);
                yield return null;
                walker.ShowAsStep(false);
            }
        }

        /// <summary>
        /// Marks the given record as visited and traces all cells back to the visited one using the records list
        /// </summary>
        /// <param name="record"></param>
        /// <param name="records"></param>
        /// <param name="service"></param>
        /// <param name="visited"></param>
        private void MarkRecordAsVisitedRecursive(KeyValuePair<MazeCell, Vector2Int> record, List<KeyValuePair<MazeCell, Vector2Int>> records, MazeGenerationService service, List<MazeCell> visited)
        {
            MazeCell cell = record.Key;
            cell.MarkAsVisited();
            visited.Add(cell);

            Vector2Int direction = record.Value;

            //create passage between cell and next
            MazeCell next = service.Cells[cell.MazeX + direction.x, cell.MazeY + direction.y];
            cell.CreatePassage(next);
            next.CreatePassage(cell);

            if (!next.IsVisited)
            {
                MarkRecordAsVisitedRecursive(records.GetRecord(next), records, service, visited);
            }
        }

        /// <summary>
        /// Returns a random unvisited cell in the cells grid
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private MazeCell GetRandomUnvisitedCell(MazeGenerationService service)
        {
            List<MazeCell> unvisited = new List<MazeCell>();

            for (int x = 0; x < service.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < service.Cells.GetLength(1); y++)
                {
                    MazeCell cell = service.Cells[x, y];
                    if (!cell.IsVisited)
                    {
                        unvisited.Add(cell);
                    }
                }
            }

            return unvisited[Random.Range(0, unvisited.Count)];
        }
    }
}