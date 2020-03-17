using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionDisaster: CardAction
{
    private DisasterType m_disasterType = DisasterType.Water;
    private int m_moveSpeed = 1;
    private Disaster m_disaster = null;

    public CardActionDisaster() : this(null) { }
    public CardActionDisaster(Player a_owner) : base(a_owner) {
        Color = Color.white;
    }
    public CardActionDisaster(DisasterType a_disasterType, int a_moveSpeed, Player a_owner = null) : base(a_owner) {
        m_disasterType = a_disasterType;
        m_moveSpeed = a_moveSpeed;
    }

    public override void Activate() {
        if (m_disaster == null) {
            if (Owner.ActiveDisaster == null) {
                Board.instance.ToggleTiles((t) => {
                    return t.IsControlledDisaster || (t.HasAdjacentOrthogonalStone(Owner) && t.IsClear);
                });
                return;
            }
            Board.instance.ToggleTiles((t) => {
                return t.IsControlledDisaster;
            });
            return;
        }
        Board.instance.ToggleTiles((t) => {
            return t.IsAdjacentOrthogonalTo(m_disaster.Head, m_moveSpeed, false) && t.IsClear;
        });
    }

    public override bool Execute(BoardTile a_tile) {
        if (m_disaster == null) {
            if (a_tile.Disaster == null) {
                Owner.CreateDisaster(m_disasterType, a_tile);
                m_disaster = Owner.ActiveDisaster;
                return false;
            }
            m_disaster = a_tile.Disaster;
            m_disaster.Advance();
            return false;
        }
        var dir = Direction.None;
        if (m_disaster.Head.x > a_tile.x)
            dir = Direction.West;
        else if (m_disaster.Head.x < a_tile.x)
            dir = Direction.East;
        else if (m_disaster.Head.y > a_tile.y)
            dir = Direction.North;
        else if (m_disaster.Head.y < a_tile.y)
            dir = Direction.South;
        Debug.Log($"Disaster dir: {dir}");
        m_disaster.Direction = dir;
        return true;
    }
}

public class CardActionWater : CardActionDisaster
{
    public CardActionWater() : base(DisasterType.Water, 1) {
        Color = Color.blue;
        Info = "Disaster ~ Water";
    }
}
