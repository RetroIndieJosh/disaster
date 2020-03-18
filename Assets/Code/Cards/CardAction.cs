using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardAction {
    public string Info = "";

    public Color Color {
        get; protected set;
    } = Color.black;

    public Player Owner {
        protected get; set;
    } = null;

    public virtual bool IsPlayable => true;

    virtual public void Activate() { }

    virtual public bool Execute(BoardTile a_tile) {
        return true;
    }

    public CardAction() { }

    public CardAction(Player a_owner) {
        Owner = a_owner;
    }

    protected void RemoveStone(BoardTile a_tile) {
        // TODO mark for death at end of card play rather than immediately
        a_tile.Controller = null;
    }

    protected void AddStone(BoardTile a_tile) {
        a_tile.Controller = Owner;
    }

    protected void CreateDisaster(DisasterType a_type, BoardTile a_tile) {
        Owner.CreateDisaster(a_type, a_tile);
    }
}

