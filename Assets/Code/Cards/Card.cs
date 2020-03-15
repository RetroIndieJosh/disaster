using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    public virtual void Activate(int a_x, int a_y, PlayerColor a_color) { }

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            if (TurnManager.instance.ActiveCard == this) {
                TurnManager.instance.ActiveCard = this;
                // TODO shift / highlight to set active
            } else {
                TurnManager.instance.ActiveCard = null;
                // TODO shift back / unhighlight to set inactive
            }
        });
    }
}
