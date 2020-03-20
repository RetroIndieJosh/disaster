using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardActionLife: CardAction
{
    public override bool IsPlayable => Board.instance.HasClearSpace;

    public CardActionLife() : this(null) { }
    public CardActionLife(Player a_owner) : base(a_owner) {
        Color = Color.green;
        Info = "Basic ~ Life\nCreate a new stone adjacent to an existing one";
        Initial = "L";
    }

    public override void Activate() {
        Board.instance.ToggleTiles((t) => {
            return t.HasAdjacentOrthogonalStone(Owner) && t.IsClear && t.IsEdge == false;
        });
    }

    public override bool Execute(BoardTile a_tile) {
        AddStone(a_tile);
        return true;
    }
}

