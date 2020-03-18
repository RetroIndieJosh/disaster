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
            if (Owner.ControlledDisaster == null) {
                // select target disaster or placement
                Board.instance.ToggleTiles((t) => {
                    return t.HasControlledDisaster || (t.HasAdjacentOrthogonalStone(Owner) && t.HasStone == false);
                });
                return;
            }
            Board.instance.ToggleTiles((t) => {
                // select our disaster (or enemy's)
                return t.HasControlledDisaster;
            });
            return;
        }
        // allowed directions 
        Board.instance.ToggleTiles((t) => {
            return t.IsAdjacentOrthogonalTo(m_disaster.Head, m_moveSpeed, false);
        });
    }

    private int m_stepsRemaining = 0;

    public override bool Execute(BoardTile a_tile) {
        if (m_disaster == null) {
            if (a_tile.Disaster == null) {
                Owner.CreateDisaster(m_disasterType, a_tile);
                m_disaster = Owner.ControlledDisaster;
                m_stepsRemaining = 1;
            } else {
                m_disaster = a_tile.Disaster;
                m_disaster.Advance();
                if (m_disaster.IsAlive == false)
                    return true;
                m_stepsRemaining = (m_disaster.DisasterType == m_disasterType) ? 2 : 1;
            }
            return false;
        }
        m_disaster.SetDirection(m_disaster.Head, a_tile);

        --m_stepsRemaining;
        if (m_stepsRemaining > 0) {
            m_disaster.Advance();
            // stop if the disaster halts advancement
            if (m_disaster.IsAlive == false)
                return true;
        }
        return m_stepsRemaining <= 0;
    }
}
