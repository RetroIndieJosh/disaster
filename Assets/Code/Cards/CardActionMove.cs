using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionMove: CardAction
{
    private BoardTile m_target = null;

    public CardActionMove() : this(null) { }
    public CardActionMove(Player a_owner) : base(a_owner) {
        Color = Color.white;
        Info = "Basic ~ Move";
    }

    public override void Activate() {
        if (m_target == null) {
            Board.instance.ToggleTiles((t) => {
                return t.StoneColor == Owner.Color;
            });
            return;
        }
        Board.instance.ToggleTiles((t) => {
            return t.IsAdjacentOrthogonalTo(m_target) && t.IsClear;
        });
    }

    public override bool Execute(BoardTile a_tile) {
        if (m_target == null) {
            m_target = a_tile;
            return false;
        }
        RemoveStone(m_target);
        AddStone(a_tile);
        return true;
    }
}

