using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionMove: CardAction
{
    public override bool IsPlayable => Board.instance.HasClearSpace;

    private BoardTile m_target = null;

    public CardActionMove() : this(null) { }
    public CardActionMove(Player a_owner) : base(a_owner) {
        Color = new Color(1f, 0.8398f, 0f);
        Info = "Basic ~ Move";
        Initial = "M";
    }

    public override void Activate() {
        if (m_target == null) {
            Board.instance.ToggleTiles((t) => {
                return t.StoneColor == Owner.Color && t.HasAdjacentClearSpace;
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

