using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DisasterType
{
    Fire,
    Plague,
    Water
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

public class Disaster
{
    public bool IsAlive => Head.HasControlledDisaster;
    public int Length => m_tileList.Count;
    public BoardTile Head => m_tileList[m_tileList.Count - 1];
    private BoardTile Tail => m_tileList[0];

    private List<BoardTile> m_tileList = new List<BoardTile>();

    public DisasterType DisasterType { get; } = DisasterType.Fire;
    public Direction Direction {
        set => Head.Direction = value;
    }

    public Disaster(DisasterType a_type, BoardTile m_startTile) {
        DisasterType = a_type;
        m_tileList.Add(m_startTile);
    }

    public void Advance() {
        if (DisasterType == DisasterType.Fire)
            StepAdvance();
        StepAdvance();
    }

    private void StepAdvance() {
        // add one past the head in its facing direction
        var controller = Head.Controller;
        Head.Controller = null;
        var next = Head.NextTile;

        // stop at water or edge of board
        if (next == null
            || (next.Disaster != null && next.Disaster.DisasterType == DisasterType.Water)) {
            controller.ClearDisaster();
            return;
        }

        // overwrite controlled fire or plague
        if (next.HasControlledDisaster 
            && (next.Disaster.DisasterType == DisasterType.Fire 
                || next.Disaster.DisasterType == DisasterType.Plague)) {
            next.Controller.ClearDisaster();
        }

        // kill stone
        if (next.HasStone)
            next.Controller.Kill();

        next.Disaster = this;
        next.Controller = controller;
        next.Direction = Head.Direction;
        m_tileList.Add(next);
    }
}

