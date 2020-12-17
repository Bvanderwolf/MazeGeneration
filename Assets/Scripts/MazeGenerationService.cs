using BWolf.MazeGeneration.Generators;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BWolf.MazeGeneration
{
    /// <summary>The maze generation service, the core of the maze generation process</summary>
    public class MazeGenerationService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Algorithm algorithm = Algorithm.RecursiveBacktracking;

        [Space]
        [SerializeField, Tooltip("start coordinates of the maze generation for generators that use it")]
        private Vector2Int start = Vector2Int.zero;

        [Space]
        [SerializeField]
        private int width = 50;

        [SerializeField]
        private int height = 50;

        [Space]
        [SerializeField, Tooltip("If slow mode is set to true, the generation will wait a video frame each step resulting in a slower process")]
        private bool slowMode = true;

        [SerializeField]
        private bool autoPlay = false;

        [SerializeField, Tooltip("If debug mode is set to true, some algorithms make cells show additional features")]
        private bool debugMode = false;

        [Header("Scene References")]
        [Space]
        [SerializeField]
        private InputField inputFieldWidth = null;

        [SerializeField]
        private InputField inputFieldHeight = null;

        [SerializeField]
        private InputField inputFieldStartX = null;

        [SerializeField]
        private InputField inputFieldStartY = null;

        [Space]
        [SerializeField]
        private Toggle toggleSlowMode = null;

        [SerializeField]
        private Toggle toggleAutoPlay = null;

        [Space]
        [SerializeField]
        private Text textAlgorithm = null;

        [Header("Project References")]
        [SerializeField]
        private GameObject prefabMazeCell = null;

        public MazeCell[,] Cells { get; private set; }

        /// <summary>
        /// The starting cell based on the stored start x and y values
        /// </summary>
        public MazeCell StartCell
        {
            get { return Cells[start.x, start.y]; }
        }

        private Dictionary<Algorithm, MazeGenerator> generators = new Dictionary<Algorithm, MazeGenerator>();

        private bool isGenerating;

        private int _width;
        private int _height;

        private void Awake()
        {
            InitializeUserInterface();
            CreateGenerators();
            Generate();
        }

        private void OnDestroy()
        {
            inputFieldWidth.onEndEdit.RemoveListener(OnWidthEndEdit);
            inputFieldHeight.onEndEdit.RemoveListener(OnHeightEndEdit);
        }

        private void OnValidate()
        {
            UpdateAlgorithmText();
            UpdateToggles();
            UpdateInputFields();
        }

        /// <summary>
        /// Sets up the input field values and listeners
        /// </summary>
        private void InitializeUserInterface()
        {
            _width = width;
            _height = height;

            inputFieldWidth.onEndEdit.AddListener(OnWidthEndEdit);
            inputFieldHeight.onEndEdit.AddListener(OnHeightEndEdit);

            inputFieldStartX.onEndEdit.AddListener(OnStartXEndEdit);
            inputFieldStartY.onEndEdit.AddListener(OnStartYEndEdit);

            toggleSlowMode.onValueChanged.AddListener(OnSlowModeToggled);
            toggleAutoPlay.onValueChanged.AddListener(OnAutoPlayToggled);
        }

        /// <summary>Creates all available generators for each algorithm</summary>
        private void CreateGenerators()
        {
            generators.Add(Algorithm.AldousBroder, new AldousBroderGenerator());
            generators.Add(Algorithm.RecursiveBacktracking, new RecursiveBacktrackingGenerator());
            generators.Add(Algorithm.BinaryTree, new BinaryTreeGenerator());
            generators.Add(Algorithm.Kruskal, new KruskalGenerator(debugMode));
            generators.Add(Algorithm.Prim, new PrimGenerator());
            generators.Add(Algorithm.Eller, new EllerGenerator(debugMode));
            generators.Add(Algorithm.Wilson, new WilsonGenerator(debugMode));
            generators.Add(Algorithm.HuntAndKil, new HuntAndKillGenerator());
            generators.Add(Algorithm.PrimsGrowingTree, new PrimsGrowingTreeGenerator());
            generators.Add(Algorithm.Sidewinder, new SidewinderGenerator());

            foreach (MazeGenerator generator in generators.Values)
            {
                generator.CompletedRoutine += OnGenerationRoutineCompleted;
            }
        }

        /// <summary>
        /// Starts the generation of the maze
        /// </summary>
        public void Generate()
        {
            if (isGenerating)
            {
                return;
            }

            if (_width < 2 || _height < 2)
            {
                Debug.LogError("Generation process halted :: The maze needs to have atleast 2 width and 2 height to be generated");
                return;
            }

            if (!(Cells != null && _width == width && _height == height))
            {
                //only if generation is called on initial generation and when input size doesn't match current size do we recreate the maze
                width = _width;
                height = _height;

                DestroyMazeCells();
                CreateMazeCells();
            }

            if (start.x < 0 || start.x >= Cells.GetLength(0) || start.y < 0 || start.y >= Cells.GetLength(1))
            {
                Debug.LogError("Generation process halted :: The start position needs to be inside the grid");
                return;
            }

            //set default values of cells before generating the maze
            SetMazeDefaultValues();

            //create the maze based on the slowMode flag and the algorithm
            if (slowMode)
            {
                isGenerating = true;
                StartCoroutine(generators[algorithm].CreateMazeRoutine(this));
            }
            else
            {
                generators[algorithm].CreateMaze(this);
            }
        }

        /// <summary>
        /// Stores given user width input
        /// </summary>
        /// <param name="value"></param>
        private void OnWidthEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _width = int.Parse(value);
            }
        }

        /// <summary>
        /// Stores given user height input
        /// </summary>
        /// <param name="value"></param>
        private void OnHeightEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _height = int.Parse(value);
            }
        }

        /// <summary>
        /// Stores given user start x input
        /// </summary>
        /// <param name="value"></param>
        private void OnStartXEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                start.x = int.Parse(value);
            }
        }

        /// <summary>
        /// Stores given user start y input
        /// </summary>
        /// <param name="value"></param>
        private void OnStartYEndEdit(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                start.y = int.Parse(value);
            }
        }

        /// <summary>
        /// Updates the slow mode function
        /// </summary>
        private void OnSlowModeToggled(bool value)
        {
            slowMode = value;
        }

        /// <summary>
        /// Updates the autoplay function
        /// </summary>
        private void OnAutoPlayToggled(bool value)
        {
            autoPlay = value;
        }

        /// <summary>
        /// Cycles through the algorithm values to set the algorithm used for generation either to the next or previous one based on given "forward" value
        /// </summary>
        /// <param name="forward"></param>
        public void CycleThroughAlgorithmValues(bool forward)
        {
            if (forward)
            {
                int next = (int)algorithm + 1;
                if (next == System.Enum.GetValues(typeof(Algorithm)).Length)
                {
                    next = 0;
                }

                algorithm = (Algorithm)next;
            }
            else
            {
                int previous = (int)algorithm - 1;
                if (previous < 0)
                {
                    previous = System.Enum.GetValues(typeof(Algorithm)).Length - 1;
                }

                algorithm = (Algorithm)previous;
            }

            UpdateAlgorithmText();
        }

        /// <summary>
        /// Updates the shown algorithm text in the User Interface
        /// </summary>
        private void UpdateAlgorithmText()
        {
            textAlgorithm.text = algorithm.ToString();
        }

        /// <summary>
        /// Updates the shown toggle states in the User Interface
        /// </summary>
        private void UpdateToggles()
        {
            toggleSlowMode.isOn = slowMode;
            toggleAutoPlay.isOn = autoPlay;
        }

        /// <summary>
        /// Updates the shown input field text in the User Interface
        /// </summary>
        private void UpdateInputFields()
        {
            inputFieldWidth.text = width.ToString();
            inputFieldHeight.text = height.ToString();

            inputFieldStartX.text = start.x.ToString();
            inputFieldStartY.text = start.y.ToString();
        }

        /// <summary>Resets the isGenerating flag</summary>
        private void OnGenerationRoutineCompleted()
        {
            isGenerating = false;

            if (autoPlay && slowMode)
            {
                CycleThroughAlgorithmValues(true);
                Generate();
            }
        }

        /// <summary>Returns a list of unvisited cells adjecent to given cell</summary>
        public List<MazeCell> GetUnVisitedNeighbours(MazeCell cell)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = GetTopNeighbour(cell);
            if (top != null && !top.IsVisited)
            {
                cells.Add(top);
            }

            MazeCell bottom = GetBottomNeighbour(cell);
            if (bottom != null && !bottom.IsVisited)
            {
                cells.Add(bottom);
            }

            MazeCell left = GetLeftNeighbour(cell);
            if (left != null && !left.IsVisited)
            {
                cells.Add(left);
            }

            MazeCell right = GetRightNeighbour(cell);
            if (right != null && !right.IsVisited)
            {
                cells.Add(right);
            }

            return cells;
        }

        /// <summary>Returns a list of visited cells adjecent to given cell</summary>
        public List<MazeCell> GetVisitedNeighbours(MazeCell cell)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = GetTopNeighbour(cell);
            if (top != null && top.IsVisited)
            {
                cells.Add(top);
            }

            MazeCell bottom = GetBottomNeighbour(cell);
            if (bottom != null && bottom.IsVisited)
            {
                cells.Add(bottom);
            }

            MazeCell left = GetLeftNeighbour(cell);
            if (left != null && left.IsVisited)
            {
                cells.Add(left);
            }

            MazeCell right = GetRightNeighbour(cell);
            if (right != null && right.IsVisited)
            {
                cells.Add(right);
            }

            return cells;
        }

        /// <summary>Returns a list of cells adjecent to given cell that are not of the same set</summary>
        public List<MazeCell> GetNeighboursNotPartOfSet(MazeCell cell)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = GetTopNeighbour(cell);
            if (top != null && top.SetNumber != cell.SetNumber)
            {
                cells.Add(top);
            }

            MazeCell bottom = GetBottomNeighbour(cell);
            if (bottom != null && bottom.SetNumber != cell.SetNumber)
            {
                cells.Add(bottom);
            }

            MazeCell left = GetLeftNeighbour(cell);
            if (left != null && left.SetNumber != cell.SetNumber)
            {
                cells.Add(left);
            }

            MazeCell right = GetRightNeighbour(cell);
            if (right != null && right.SetNumber != cell.SetNumber)
            {
                cells.Add(right);
            }

            return cells;
        }

        // <summary>Returns a list of cells adjecent to given cell</summary>
        public List<MazeCell> GetNeighbours(MazeCell cell)
        {
            List<MazeCell> cells = new List<MazeCell>();

            MazeCell top = GetTopNeighbour(cell);
            if (top != null)
            {
                cells.Add(top);
            }

            MazeCell bottom = GetBottomNeighbour(cell);
            if (bottom != null)
            {
                cells.Add(bottom);
            }

            MazeCell left = GetLeftNeighbour(cell);
            if (left != null)
            {
                cells.Add(left);
            }

            MazeCell right = GetRightNeighbour(cell);
            if (right != null)
            {
                cells.Add(right);
            }

            return cells;
        }

        /// <summary>
        /// Returns the neighbour above given cell. Will return null if the neighbour was outside of the grid
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public MazeCell GetTopNeighbour(MazeCell cell)
        {
            //only returns the top maze cell relative to given x,y coordinates if y coordinate is inside the maze y dimension
            return cell.MazeY + 1 < Cells.GetLength(1) ? Cells[cell.MazeX, cell.MazeY + 1] : null;
        }

        /// <summary>
        /// Returns the neighbour below given cell. Will return null if the neighbour was outside of the grid
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public MazeCell GetBottomNeighbour(MazeCell cell)
        {
            //only returns the top maze cell relative to given x,y coordinates if y coordinate is inside the maze y dimension
            return cell.MazeY - 1 >= 0 ? Cells[cell.MazeX, cell.MazeY - 1] : null;
        }

        /// <summary>
        /// Returns the neighbour to the left of given cell. Will return null if the neighbour was outside of the grid
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public MazeCell GetLeftNeighbour(MazeCell cell)
        {
            //only returns the top maze cell relative to given x,y coordinates if x coordinate is inside the maze x dimension
            return cell.MazeX - 1 >= 0 ? Cells[cell.MazeX - 1, cell.MazeY] : null;
        }

        /// <summary>
        /// Returns the neighbour to the right of given cell. Will return null if the neighbour was outside of the grid
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public MazeCell GetRightNeighbour(MazeCell cell)
        {
            //only returns the top maze cell relative to given x,y coordinates if x coordinate is inside the maze x dimension
            return cell.MazeX + 1 < Cells.GetLength(0) ? Cells[cell.MazeX + 1, cell.MazeY] : null;
        }

        /// <summary>Creates a grid full of maze cells based on given width and heigth values</summary>
        private void CreateMazeCells()
        {
            Cells = new MazeCell[width, height];

            Camera camera = Camera.main;
            Bounds cellBounds = prefabMazeCell.GetComponent<SpriteRenderer>().bounds;
            Vector3 cellScale = prefabMazeCell.transform.localScale;

            Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
            Vector3 topLeft = camera.ScreenToWorldPoint(new Vector3(0, Screen.height));
            Vector3 bottomRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, 0));

            float screenWidth = (bottomLeft - bottomRight).magnitude;
            float screenHeight = (topLeft - bottomLeft).magnitude;

            float cellWidth = cellBounds.size.x;
            float cellHeight = cellBounds.size.y;

            float gridWidth = cellWidth * width;
            float gridHeight = cellHeight * height;

            //scale cells down if grid width is greater than screen width
            if (gridWidth > screenWidth)
            {
                cellScale *= (screenWidth / gridWidth);

                cellWidth = cellBounds.size.x * cellScale.x;
                cellHeight = cellBounds.size.y * cellScale.y;

                gridWidth = cellWidth * width;
                gridHeight = cellHeight * height;
            }

            //scale cells down if grid height is greater than screen height after first reduction
            if (gridHeight > screenHeight)
            {
                cellScale *= (screenHeight / gridHeight);

                cellWidth = cellBounds.size.x * cellScale.x;
                cellHeight = cellBounds.size.y * cellScale.y;

                gridWidth = cellWidth * width;
                gridHeight = cellHeight * height;
            }

            //scale cells up if the screen size is bigger than the grid size
            if (gridWidth < screenWidth && gridHeight < screenHeight)
            {
                //use lowest percentage of width and height to scale cells up
                cellScale *= Mathf.Min((screenHeight / gridHeight), (screenWidth / gridWidth));

                cellWidth = cellBounds.size.x * cellScale.x;
                cellHeight = cellBounds.size.y * cellScale.y;

                gridWidth = cellWidth * width;
                gridHeight = cellHeight * height;
            }

            //assign left and bottom margin values based on leftover space between screen and grid
            float left = (screenWidth - gridWidth) * 0.5f;
            float bottom = (screenHeight - gridHeight) * 0.5f;

            //set origin of grid at bottom left, including compensation for pivot of cell being at the center
            Vector3 origin = bottomLeft + Vector3.Scale(cellBounds.extents, cellScale);

            //create the grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xPos = left + x * cellWidth;
                    float yPos = bottom + y * cellWidth;
                    Cells[x, y] = CreateMazeCell(origin.x + xPos, origin.y + yPos, x, y, cellScale);
                }
            }
        }

        /// <summary>
        /// Destroys all cells part of the grid
        /// </summary>
        private void DestroyMazeCells()
        {
            if (Cells != null)
            {
                foreach (MazeCell cell in Cells)
                {
                    Destroy(cell.gameObject);
                }

                Cells = null;
            }
        }

        /// <summary>
        /// Sets the default values for all cells in the grid
        /// </summary>
        private void SetMazeDefaultValues()
        {
            foreach (MazeCell cell in Cells)
            {
                cell.SetDefaultValues();
            }
        }

        /// <summary>
        /// Returns a new maze cell created at given coordinates
        /// </summary>
        /// <param name="worldX">x position in world space</param>
        /// <param name="worldY">y position in world space</param>
        /// <param name="mazeX">x position of cell in maze grid</param>
        /// <param name="mazeY">y position of cell in maze grid</param>
        /// <returns></returns>
        private MazeCell CreateMazeCell(float worldX, float worldY, int mazeX, int mazeY, Vector3 scale)
        {
            MazeCell cell = Instantiate(prefabMazeCell).GetComponent<MazeCell>();
            cell.transform.position = new Vector3(worldX, worldY);
            cell.transform.localScale = scale;
            cell.name = $"cell ({mazeX}, {mazeY})";

            //store maze coordinates to make creating passages and checking neighbours easier
            cell.SetMazeCoordinates(mazeX, mazeY);

            return cell;
        }

        /// <summary>
        /// The Algorithms that can be used for generating a maze
        /// </summary>
        private enum Algorithm
        {
            RecursiveBacktracking,
            AldousBroder,
            BinaryTree,
            Kruskal,
            Prim,
            Eller,
            Wilson,
            HuntAndKil,
            PrimsGrowingTree,
            Sidewinder
        }
    }
}