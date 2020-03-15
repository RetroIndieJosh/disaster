using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoardTileState
{
    Clear = 0,
    Black = 1,
    White = 2,
    Fire = 3,
    Flood = 4,
    Plague = 5
}

public enum Direction
{
    None = 0x00,
    East = 0x01,
    North = 0x02,
    South = 0x04,
    West = 0x08,
    Northeast = 0x02 + 0x01,
    Northwest = 0x02 + 0x08,
    Southeast = 0x04 + 0x01,
    Southwest = 0x04 + 0x08
}

public class BoardTile : MonoBehaviour
{
    [SerializeField] private Image m_overlayImage = null;

    Player m_controller = null;
    Disaster m_disaster = null;

    public bool IsClear => m_controller == null && m_disaster == null;

    public void Clear() {
        m_controller = null;
        SetOverlay(null);
    }

    public void SetStone(Player a_player) {
        Debug.Log($"Player {a_player} sets stone");
        m_controller = a_player;
        var color = a_player.Color;
        var sprite = TurnManager.instance.GetStoneSprite(color);
        SetOverlay(sprite);
    }

    private void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            TurnManager.instance.ActivateCard(this);
        });
    }

    private void SetOverlay(Sprite a_sprite) {
        m_overlayImage.sprite = a_sprite;
        m_overlayImage.enabled = a_sprite != null;
    }
}
