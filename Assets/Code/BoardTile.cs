using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    None = 0x00,
    East = 0x01,
    North = 0x02,
    South = 0x04,
    West = 0x08,
    Northeast = 0x02 + 0x01,
    Northwest = 0x02 + 0x08,
    Southeast = 0x04 + 0x01,
    Southwest = 0x04 + 0x08
}

public class BoardTile : MonoBehaviour
{
    private Direction m_facing = Direction.None;

    private void Activate() {
    }

    private void Start() {
        var button = GetComponent<Button>();
    }
}
