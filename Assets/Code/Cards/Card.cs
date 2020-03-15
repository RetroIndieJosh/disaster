using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    public virtual void Activate(BoardTile a_tile, Player a_player) { }

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            TurnManager.instance.ActiveCard = this;
            Debug.Log($"Card [{name}] active");
            // TODO how to tell if active?
                // TODO shift / highlight to set active
                // TODO shift back / unhighlight to set inactive
        });
    }
}
