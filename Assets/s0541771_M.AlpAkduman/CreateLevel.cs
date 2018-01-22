using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.s0541771_M.AlpAkduman
{
    public class CreateLevel : MonoBehaviour
    {
        public GameObject Ball;
        public int XHalfExt = 1;
        public int ZHalfExt = 1;
        public Text message;
        public GameObject OuterWall;
        public GameObject InnerWall;
        public GameObject ExitTile;
        public GameObject[] FloorTiles;

        //private int xExt, zExt;
        //private int start, end;

        public int MazeRows, MazeColumns;
        public GameObject Floor, Environment, Root;
        public float TileSize = 4f;

        private List<Cell> _cells;
        private int _floorTilesCount; 
        private int _randomTilesIndex; 



        // Use this for initialization
        void Awake()
        {
            //Gather together all refrences you will need later on
            _floorTilesCount = FloorTiles.Length;


            Root = GameObject.Find("MovablePlayfield");
            Root.transform.localPosition = new Vector3(0,-3,0);
            if (Root != null)
            {
                //Create the all walls for given extXZ                
                //Build the maze from the given set of prefabs
                //Set the walls for the maze (place only one wall between two cells, not two!)
                CreateAllWalls(Root);

                //create a maze
                var ma = new MazeCreator(_cells, XHalfExt, ZHalfExt, ExitTile);
                ma.CreateMaze();

                //Place the PlayerBall above the playfiel
                PlaceBallStart();
                //Scale  + position BasePlate
                Scale();
            }
        }

        //You might need this more than once...
         private void PlaceBallStart()
        {
            //Reset Physics
            //Place the ball
            Ball.transform.localPosition = new Vector3(0, 12, 0);
        }

        public void EndZoneTrigger(GameObject other)
        {
            if (other.gameObject.CompareTag("Lost"))
            {
                message.text = "You Lose!";
                PlaceBallStart();
            }
            //Check if ball first...
            //Player has fallen onto ground plane, reset
        }
        public void WinTrigger(GameObject other)
        {
            //Check if ball first...
            if (other.gameObject.CompareTag("Win"))
            {
                message.text = "You Win!";           
                //Destroy this maze
                DestroyMaze();
                //Generate new maze
                Awake();
                //Player has fallen onto ground plane, reset
            }

        }

        public void DestroyMaze()
        {
            foreach (var cell in _cells)
            {
                DestroyObject(cell.Floor);
                DestroyObject(cell.NorthWall);
                DestroyObject(cell.EastWall);
                DestroyObject(cell.SouthWall);
                DestroyObject(cell.WestWall);
            }
        }
        public void Scale()
        {



            //Build the values for xExt and zExt from xHalfExt and zHalfExt
            //Build an offset for the dyn playfield from the BasePlatform e.g. the bigger halfExtent value in unity units
            var mazeXSize = TileSize * ((2*XHalfExt)+1);
            var mazeZSize = TileSize * ((2 * ZHalfExt) + 1);
            //Calculate a scale factor for scaling the non-movable environment (and therefore the camera) and the BasePlatform 
            // the factors that the environment are scaled for right now are for x/zHalfExt =1, scale accordingly
            //i.e. the playfield/environment should be as big as the dynamic field
            var xScale = (XHalfExt / 3f);
            var zScale = (ZHalfExt / 3f);
            //padding
            var xPadding = (xScale / 100f) * 10f;
            var zPadding = (zScale / 100f) * 10f;
            var xScaleWithPadding = xScale + xPadding;
            var zScaleWithPadding = zScale + zPadding;

            //Scale Environment
            Environment.transform.localScale = new Vector3(xScaleWithPadding,1, zScaleWithPadding);
            var basement = GameObject.Find("DSBasementFloor");
            basement.transform.localScale = new Vector3(mazeXSize, 1, mazeZSize);
            basement.transform.parent = Root.transform;

        }

        //Create all walls 
        private void CreateAllWalls(GameObject root)
        {
            _cells = new List<Cell>();

            //Create all Floor and Walls for a new Cell
            for (var r = -XHalfExt; r <= XHalfExt; r++)
            {
                for (var c = -ZHalfExt; c <= ZHalfExt; c++)
                {
                    //To avoid a hole at start point
                    if(r == 0 && c == 0)
                        Floor = ExitTile;
                        //Floor = FloorTiles[1];
                    else
                    {
                        _randomTilesIndex = Random.Range(0, _floorTilesCount);
                        Floor = FloorTiles[_randomTilesIndex];
                    }

                    var cell = new Cell()
                    {
                        Floor = Instantiate(Floor, new Vector3(r * TileSize, 3, c * TileSize), Quaternion.identity, root.transform),
                        Position = new Vector2(r,c)
                    };

                    if (c == -ZHalfExt)
                    {
                        cell.WestWall = Instantiate(OuterWall, new Vector3(r * TileSize, 3.5f, (c * TileSize) - (TileSize / 2f)),
                            Quaternion.identity, root.transform);
                    }

                    if (c == ZHalfExt)
                    {
                        cell.EastWall = Instantiate(OuterWall, new Vector3(r * TileSize, 3.5f, (c * TileSize) + (TileSize / 2f)), Quaternion.identity, root.transform);
                    }

                    else
                    {
                        cell.EastWall = Instantiate(InnerWall, new Vector3(r * TileSize, 2.5f, (c * TileSize) + (TileSize / 2f)), Quaternion.identity, root.transform);
                    }

                    if (r == -XHalfExt)
                    {
                        cell.NorthWall = Instantiate(OuterWall, new Vector3((r * TileSize) - (TileSize / 2f), 3.5f, c * TileSize), Quaternion.Euler(0, 90, 0),root.transform);
                    }

                    if (r == XHalfExt)
                    {
                        cell.SouthWall = Instantiate(OuterWall, new Vector3((r * TileSize) + (TileSize / 2f), 3.5f, c * TileSize), Quaternion.Euler(0, 90, 0),root.transform);
                    }

                    else
                    {
                        cell.SouthWall = Instantiate(InnerWall, new Vector3((r * TileSize) + (TileSize / 2f), 2.5f, c * TileSize), Quaternion.Euler(0, 90, 0), root.transform);
                    }

                    _cells.Add(cell);

                }
            }

        }

    }
}


