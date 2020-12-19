using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class KruskalGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using Kruskal's algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            //assign each maze cell a number indicating which set it belongs to
            AssignSetNumbers(service.Cells, service.DebugMode);

            //create data structures necessary for algorithm to work
            MazeCell[] cells = service.Cells.Cast<MazeCell>().ToArray();
            List<MazeCell> bag = new List<MazeCell>(cells);

            //loop while bag is not not empty
            while (bag.Count != 0)
            {
                //pick a random cell and mark it as visited
                MazeCell randomCell = bag[Random.Range(0, bag.Count)];
                randomCell.MarkAsVisited();

                //fetch the neigbours of this cell that are not part of the same set
                List<MazeCell> neighbours = service.GetNeighboursNotPartOfSet(randomCell);

                if (neighbours.Count > 0)
                {
                    //pick a random neighbour and mark it as visited
                    MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                    randomNeighbour.MarkAsVisited();

                    //create passage between random cell and neighbour
                    randomNeighbour.CreatePassage(randomCell);
                    randomCell.CreatePassage(randomNeighbour);

                    //all the cells belonging to the set the random neighbour belongs to are overtaken by the random cell's set
                    MazeCell[] set = cells.Where(c => c.SetNumber == randomNeighbour.SetNumber).ToArray();
                    foreach (MazeCell overtakableCell in set)
                    {
                        overtakableCell.SetSetNumber(randomCell.SetNumber);
                    }
                }
                else
                {
                    //if all neighbours of the cel are part of the same set, remove the cell from the bag
                    bag.Remove(randomCell);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that creates a maze by linking cells using Kruskal's algorithm
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            //assign each maze cell a number indicating which set it belongs to
            AssignSetNumbers(service.Cells, service.DebugMode);

            //create data structures necessary for algorithm to work
            MazeCell[] cells = service.Cells.Cast<MazeCell>().ToArray();
            List<MazeCell> bag = new List<MazeCell>(cells);

            //loop while bag is not not empty
            while (bag.Count != 0)
            {
                //pick a random cell and mark it as visited
                MazeCell randomCell = bag[Random.Range(0, bag.Count)];
                randomCell.MarkAsVisited();

                //fetch the neigbours of this cell that are not part of the same set
                List<MazeCell> neighbours = service.GetNeighboursNotPartOfSet(randomCell);

                if (neighbours.Count > 0)
                {
                    //pick a random neighbour and mark it as visited
                    MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                    randomNeighbour.MarkAsVisited();

                    //link the random neighbour with the random cell
                    randomNeighbour.CreatePassage(randomCell);
                    randomCell.CreatePassage(randomNeighbour);

                    //all the cells belonging to the set the random neighbour belongs to are overtaken by the random cell's set
                    MazeCell[] set = cells.Where(c => c.SetNumber == randomNeighbour.SetNumber).ToArray();
                    foreach (MazeCell overtakableCell in set)
                    {
                        overtakableCell.SetSetNumber(randomCell.SetNumber);
                    }
                }
                else
                {
                    //if all neighbours of the cel are part of the same set, remove the cell from the bag
                    bag.Remove(randomCell);
                }

                yield return null;
            }

            CompletedRoutine();
        }

        /// <summary>
        /// Assigns given cells a set number based on their position
        /// </summary>
        /// <param name="cells"></param>
        private void AssignSetNumbers(MazeCell[,] cells, bool debugMode)
        {
            int count = 0;
            foreach (MazeCell cell in cells)
            {
                cell.SetSetNumber(count++);
                cell.ShowSetNumber(debugMode);
            }
        }
    }
}