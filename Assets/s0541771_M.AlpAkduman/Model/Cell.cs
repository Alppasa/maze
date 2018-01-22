using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool Visited = false;
    public GameObject NorthWall, SouthWall, EastWall, WestWall, Floor;
    public Vector2 Position;
    public int X;
    public int Z;
}
