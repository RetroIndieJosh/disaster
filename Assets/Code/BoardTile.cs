using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardTileState
{
    Clear = 0,
    Black = 1,
    White = 2,
    Fire = 3,
    Flood = 4,
    Plague = 5
}

public enum Direction
{
    None = 0,
    East = 1,
    North = 2,
    Northeast = 3,
    Northwest = 4,
    South = 5,
    Southeast = 6,
    Southwest = 7,
    West = 8
}

public enum PlayerColor {
    None = 0,
    Black = 1,
    White = 2
}

public class BoardTile : MonoBehaviour
{
    private Direction m_facing = Direction.None;
    private PlayerColor m_controller = PlayerColor.None;
}
