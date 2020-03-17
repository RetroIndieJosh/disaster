using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardAction {
    public Color Color {
        get; protected set;
    } = Color.black;

    public Player Owner {
        protected get; set;
    } = null;

    virtual public void Activate() { }

    virtual public bool Execute(BoardTile a_tile) {
        return true;
    }

    public CardAction() { }

    public CardAction(Player a_owner) {
        Owner = a_owner;
    }

    protected void Death(BoardTile a_tile) {
        // TODO mark for death at end of card play rather than immediately
        a_tile.Controller = null;
    }

    protected void Life(BoardTile a_tile) {
        a_tile.Controller = Owner;
    }
}

