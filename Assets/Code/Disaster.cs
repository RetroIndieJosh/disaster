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

    public void Advance(Player a_owner) {
        // add one past the head in its facing direction
        Head.Controller = null;
        var next = Head.NextTile();
        if (next == null
            || (next.Disaster != null && next.Disaster.DisasterType == DisasterType.Water)) {
            a_owner.ClearDisaster();
            return;
        }
        if (next.StoneColor != PlayerColor.None)
            next.Controller.Kill();
        next.Disaster = this;
        next.Controller = a_owner;
        m_tileList.Add(next);
    }
}

