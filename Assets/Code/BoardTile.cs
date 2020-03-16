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

public class BoardTile : GameElement
{
    [SerializeField] private Image m_overlayImage = null;

    [HideInInspector] public int x = -1;
    [HideInInspector] public int y = -1;

    public BoardTileState State {
        get => m_state;
        set {
            m_state = value;
            UpdateInfo();
        }
    }

    private Player m_controller = null;
    private Disaster m_disaster = null;
    private BoardTileState m_state = BoardTileState.Clear;

    public void Clear() {
        m_controller = null;
        m_disaster = null;
        m_state = BoardTileState.Clear;
        SetOverlay(null);
    }

    public bool IsAdjacentOrthogonalTo(BoardTile a_tile) {
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_tile != this && (
            (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 0)
            || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) == 1));
    }

    public bool HasAdjacentOrthogonalStone(Player a_player) {
        var state = (a_player.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.White;
        return Board.instance.CheckNeighborsOrthogonal(x, y, state) && m_state != state;
    }

    public void SetStone(Player a_player) {
        Debug.Log($"Player {a_player} sets stone");
        m_controller = a_player;
        State = (a_player.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.White;
        var sprite = Board.instance.GetStoneSprite(a_player.Color);
        SetOverlay(sprite);
    }

    private void Start() {
        UpdateInfo();

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            Board.instance.ActivateCard(this);
        });
    }

    private void SetOverlay(Sprite a_sprite) {
        m_overlayImage.sprite = a_sprite;
        m_overlayImage.enabled = a_sprite != null;
    }

    private void UpdateInfo() {
        m_infoText = $"Board {x},{y}: {m_state}";
        if( m_controller != null)
            m_infoText += $"(Controlled by {m_controller.name})";
    }
}
