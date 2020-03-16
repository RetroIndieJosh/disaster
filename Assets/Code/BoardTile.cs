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

    [HideInInspector] public int x = -1;
    [HideInInspector] public int y = -1;

    public BoardTileState State => m_state;

    private Player m_controller = null;
    private Disaster m_disaster = null;
    private BoardTileState m_state = BoardTileState.Clear;

    public void Clear() {
        m_controller = null;
        SetOverlay(null);
    }

    public bool HasAdjacentOrthogonalStone(Player a_player) {
        var state = (a_player.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.White;
        return Board.instance.CheckNeighborsOrthogonal(x, y, state) && m_state != state;
    }

    public void SetStone(Player a_player) {
        Debug.Log($"Player {a_player} sets stone");
        m_controller = a_player;
        m_state = (a_player.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.Clear;
        var sprite = Board.instance.GetStoneSprite(a_player.Color);
        SetOverlay(sprite);
    }

    private void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            Board.instance.ActivateCard(this);
        });
    }

    private void SetOverlay(Sprite a_sprite) {
        m_overlayImage.sprite = a_sprite;
        m_overlayImage.enabled = a_sprite != null;
    }
}
