using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardTile : GameElement
{
    [SerializeField] private Image m_overlayImage = null;

    [HideInInspector] public int x = -1;
    [HideInInspector] public int y = -1;

    public bool IsClear => m_controller == null && m_disaster == null;
    public bool IsControlledDisaster => m_controller != null && m_disaster != null;
    public PlayerColor StoneColor 
        => m_controller == null || m_disaster != null
            ? PlayerColor.None
            : m_controller.Color;

    public Disaster Disaster {
        get => m_disaster;
        set {
            m_disaster = value;
            UpdateOverlay();
        }
    }

    public Player Controller {
        get => m_controller;
        set {
            m_controller = value;
            UpdateOverlay();
        }
    }

    private Player m_controller = null;
    private Disaster m_disaster = null;

    public void Clear() {
        m_controller = null;
        m_disaster = null;
        UpdateOverlay();
    }

    public bool IsAdjacentDiagonalTo(BoardTile a_tile, int a_distance = 1) {
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_tile != this && Mathf.Abs(dx) == Mathf.Abs(dy) && Mathf.Abs(dx) <= a_distance
            && Mathf.Abs(dy) <= a_distance;
    }

    public bool IsAdjacentOrthogonalTo(BoardTile a_tile, int a_distance = 1, bool a_includeBetween = false) {
        if (a_tile == this) return false;
        var dx = a_tile.x - x;
        var dy = a_tile.y - y;
        return a_includeBetween
            ? (Mathf.Abs(dx) <= a_distance && Mathf.Abs(dy) == 0)
                || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) <= a_distance)
            : (Mathf.Abs(dx) == a_distance && Mathf.Abs(dy) == 0)
                || (Mathf.Abs(dx) == 0 && Mathf.Abs(dy) == a_distance);
    }

    public bool HasAdjacentOrthogonalStone(Player a_player) {
        return Board.instance.CheckNeighborsOrthogonal(x, y, a_player.Color) && IsClear;
    }

    private Direction m_direction = Direction.None;
    public Direction Direction {
        set {
            m_direction = value;
            UpdateOverlay();
        }
    }

    public BoardTile NextTile() {
        var dx = 0;
        var dy = 0;
        if (m_direction == Direction.East || m_direction == Direction.Northeast || m_direction == Direction.Southeast)
            dx = 1;
        if (m_direction == Direction.West || m_direction == Direction.Northwest || m_direction == Direction.Southwest)
            dx = -1;
        if (m_direction == Direction.North || m_direction == Direction.Northeast || m_direction == Direction.Northwest)
            dy = -1;
        if (m_direction == Direction.South || m_direction == Direction.Southeast || m_direction == Direction.Southwest)
            dy = 1;
        return Board.instance.GetTile(x + dx, y + dy);
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
        m_infoText = $"Board {x},{y}: ";
        if (IsClear)
            m_infoText += "Clear";
        else {
            if (m_controller != null)
                m_infoText += $"{m_controller.Color} ";
            if (m_disaster != null)
                m_infoText += $"{m_disaster.DisasterType}";
        }
    }

    private void UpdateOverlay() {
        Sprite sprite = null;
        if (StoneColor != PlayerColor.None) {
            sprite = Board.instance.GetStoneSprite(StoneColor);
        } else if (m_disaster != null) {
            sprite = m_controller == null
                ? Board.instance.GetDisasterSprite(m_disaster.DisasterType)
                : Board.instance.GetDisasterSprite(m_disaster.DisasterType, m_controller.Color, 
                    m_direction == Direction.None);

            m_overlayImage.transform.rotation = Quaternion.identity;
            if (m_direction == Direction.North)
                m_overlayImage.transform.Rotate(Vector3.forward, 0f);
            else if (m_direction == Direction.Northwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 45f);
            else if (m_direction == Direction.West)
                m_overlayImage.transform.Rotate(Vector3.forward, 90f);
            else if (m_direction == Direction.Southwest)
                m_overlayImage.transform.Rotate(Vector3.forward, 135f);
            else if (m_direction == Direction.South)
                m_overlayImage.transform.Rotate(Vector3.forward, 180f);
            else if (m_direction == Direction.Southeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 225f);
            else if (m_direction == Direction.East)
                m_overlayImage.transform.Rotate(Vector3.forward, 270f);
            else if (m_direction == Direction.Northeast)
                m_overlayImage.transform.Rotate(Vector3.forward, 315f);
        }
        SetOverlay(sprite);
    }
}
