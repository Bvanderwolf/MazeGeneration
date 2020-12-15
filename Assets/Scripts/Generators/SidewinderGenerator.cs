using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Generators
{
    public class SidewinderGenerator : MazeGenerator
    {
        /// <summary>
        /// Creates a maze by linking cells using the Sidewinder algorithm
        /// </summary>
        /// <param name="service"></param>
        public override void CreateMaze(MazeGenerationService service)
        {
            MazeCell[,] cells = service.Cells;
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            //loop through all the grid, row for row
            for (int y = height - 1; y >= 0; y--)
            {
                //setup an empty set for each row
                List<MazeCell> set = new List<MazeCell>();
                for (int x = 0; x < width; x++)
                {
                    //mark the cell it the row as visited and add it to the set
                    MazeCell cell = cells[x, y];
                    cell.MarkAsVisited();
                    set.Add(cell);

                    //make random decision for cells not on the top
                    if (y + 1 == height || Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        if (x + 1 < width)
                        {
                            //if the cell east of this cell is not out of width bounds, link the cell with it
                            MazeCell right = cells[x + 1, y];
                            cell.Link(right);
                            right.Link(cell);
                        }
                        else if (y + 1 < height)
                        {
                            //if the cell east of this cell is out of width bounds and this is not the top row, link the cell with the cell north of it
                            MazeCell randomCell = set[Random.Range(0, set.Count)];
                            MazeCell top = cells[randomCell.MazeX, y + 1];

                            randomCell.Link(top);
                            top.Link(randomCell);
                        }
                    }
                    else
                    {
                        //if the random decision was not to carve east, carve north from a random cell in the set
                        MazeCell randomCell = set[Random.Range(0, set.Count)];
                        MazeCell top = cells[randomCell.MazeX, y + 1];

                        randomCell.Link(top);
                        top.Link(randomCell);

                        //clear the set after carving north
                        set.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Returns an Enumerator that creates a maze by linking cells using the Sidewinder algorithm
        /// </summary>
        /// <param name="service"></param>
        public override IEnumerator CreateMazeRoutine(MazeGenerationService service)
        {
            MazeCell[,] cells = service.Cells;
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            //loop through all the grid, row for row
            for (int y = height - 1; y >= 0; y--)
            {
                //setup an empty set for each row
                List<MazeCell> set = new List<MazeCell>();
                for (int x = 0; x < width; x++)
                {
                    //mark the cell it the row as visited and add it to the set
                    MazeCell cell = cells[x, y];
                    cell.MarkAsVisited();
                    set.Add(cell);

                    //make random decision for cells not on the top
                    if (y + 1 == height || Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        if (x + 1 < width)
                        {
                            //if the cell east of this cell is not out of width bounds, link the cell with it
                            MazeCell right = cells[x + 1, y];
                            cell.Link(right);
                            right.Link(cell);
                        }
                        else if (y + 1 < height)
                        {
                            //if the cell east of this cell is out of width bounds and this is not the top row, link the cell with the cell north of it
                            MazeCell randomCell = set[Random.Range(0, set.Count)];
                            MazeCell top = cells[randomCell.MazeX, y + 1];

                            randomCell.Link(top);
                            top.Link(randomCell);
                        }
                    }
                    else
                    {
                        //if the random decision was not to carve east, carve north from a random cell in the set
                        MazeCell randomCell = set[Random.Range(0, set.Count)];
                        MazeCell top = cells[randomCell.MazeX, y + 1];

                        randomCell.Link(top);
                        top.Link(randomCell);

                        //clear the set after carving north
                        set.Clear();
                    }

                    yield return null;
                }
            }

            CompletedRoutine();
        }
    }
}