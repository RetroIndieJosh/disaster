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
    Water = 4,
    Plague = 5
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
    private BoardTileState m_state = BoardTileState.Clear;

    public void Clear() {
        m_controller = null;
        m_state = BoardTileState.Clear;
        SetOverlay(null);
    }

    public bool IsAdjacentDiagonalTo(BoardTile a_tile, int a_distance = 1) {
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_tile != this && Mathf.Abs(dx) == Mathf.Abs(dy) && Mathf.Abs(dx) <= a_distance 
            && Mathf.Abs(dy) <= a_distance;
    }

    public bool IsAdjacentOrthogonalTo(BoardTile a_tile, int a_distance = 1) {
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_tile != this && (
            (Mathf.Abs(dx) <= a_distance && Mathf.Abs(dy) == 0)
            || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) <= a_distance));
    }

    public bool HasAdjacentOrthogonalStone(Player a_player) {
        var state = (a_player.Color == PlayerColor.Black) ? BoardTileState.Black : BoardTileState.White;
        return Board.instance.CheckNeighborsOrthogonal(x, y, state) && m_state != state;
    }

    private Direction m_direction = Direction.None;
    public Direction Direction {
        set {
            m_direction = value;
            m_overlayImage.transform.rotation = Quaternion.identity;
            if( m_direction == Direction.North)
                m_overlayImage.transform.Rotate(Vector3.forward, 0f);
            else if (m_direction == Direction.Northwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 45f);
            else if( m_direction == Direction.West)
                m_overlayImage.transform.Rotate(Vector3.forward, 90f);
            else if (m_direction == Direction.Southwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 135f);
            else if( m_direction == Direction.South)
                m_overlayImage.transform.Rotate(Vector3.forward, 180f);
            else if( m_direction == Direction.Southeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 225f);
            else if( m_direction == Direction.East)
                m_overlayImage.transform.Rotate(Vector3.forward, 270f);
            else if( m_direction == Direction.Northeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 315f);
        }
    }

    public BoardTile NextTile() {
        var dx = 0;
        var dy = 0;
        if (m_direction == Direction.East || m_direction == Direction.Northeast || m_direction == Direction.Southeast)
            dx = -1;
        if (m_direction == Direction.West || m_direction == Direction.Northwest || m_direction == Direction.Southwest)
            dx = 1;
        if (m_direction == Direction.North || m_direction == Direction.Northeast || m_direction == Direction.Northwest)
            dy = -1;
        if (m_direction == Direction.South || m_direction == Direction.Southeast || m_direction == Direction.Southwest)
            dy = 1;
        return Board.instance.GetTile(x + dx, y + dy);
    }

    public void SetDisaster(Player a_player, DisasterType a_disaster) {
        m_controller = a_player;
        if (a_disaster == DisasterType.Fire)
            State = BoardTileState.Fire;
        else if (a_disaster == DisasterType.Plague)
            State = BoardTileState.Plague;
        else if (a_disaster == DisasterType.Water)
            State = BoardTileState.Water;
        var sprite = Board.instance.GetDisasterSprite(a_disaster);
        SetOverlay(sprite);
    }

    public void SetStone(Player a_player) {
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
