using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    None = 0x00,
    Move = 0x01, // move up to two
    Life = 0x02, // add one
    Step = 0x04, // advance one
    Spread = 0x08, // advance two
    Fire = 0x0F,
    Water = 0x10,
    Plague = 0x11,
    Wild = 0x12,
    Reserved1 = 0x14,
    Reserved2 = 0x18,
    Reserved3 = 0x1F
}

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    Button m_button = null;
    private CardType m_type = CardType.None;
    public CardType CardType {
        get => m_type;
        set {
            m_type = value;
            UpdateColor();
        }
    }

    public virtual void Activate(BoardTile a_tile, Player a_player) {
        a_tile.SetStone(a_player);
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Start() {
        m_button.onClick.AddListener(() => {
            TurnManager.instance.ActiveCard = this;
            Debug.Log($"Card [{name}] active");
            // TODO how to tell if active?
                // TODO shift / highlight to set active
                // TODO shift back / unhighlight to set inactive
        });
    }

    private void UpdateColor() {
        var colors = m_button.colors;
        switch (m_type) {
        case CardType.Fire:
            colors.normalColor = Color.red;
            break;
        case CardType.Life:
            colors.normalColor = Color.green;
            break;
        case CardType.Move:
            colors.normalColor = Color.black;
            break;
        case CardType.None:
            colors.normalColor = Color.white;
            break;
        case CardType.Plague:
            colors.normalColor = Color.yellow;
            break;
        case CardType.Spread:
            colors.normalColor = Color.gray;
            break;
        case CardType.Step:
            colors.normalColor = Color.gray;
            break;
        case CardType.Water:
            colors.normalColor = Color.blue;
            break;
        case CardType.Wild:
            colors.normalColor = new Color(0.8f, 0.0f, 0.5f);
            break;
        }
        m_button.colors = colors;
        Debug.Log($"Set card {CardType} color to {colors.normalColor}");
    }
}
