using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardTile : GameElement
{
    [SerializeField] private Image m_image = null;
    [SerializeField] private Image m_overlayImage = null;

    [HideInInspector] public int x = -1;
    [HideInInspector] public int y = -1;

    public bool IsClear => m_controller == null && m_disaster == null;
    public bool IsEdge => Board.instance.IsEdge(x, y);
    public bool HasStone => m_controller != null && m_disaster == null;
    public bool HasControlledDisaster => m_controller != null && m_disaster != null;
    public PlayerColor StoneColor
        => m_controller == null || m_disaster != null
            ? PlayerColor.None
            : m_controller.Color;
    public Color TileColor {
        get => m_image.color;
        set => m_image.color = value;
    }

    // TODO account for adjacent disasters (block but still controlled by none)
    public bool HasAdjacentDiagonalClearSpace(int a_distance) {
        var neighbors = Board.instance.GetNeighborsDiagonal(x, y, a_distance);
        foreach (var neighbor in neighbors)
            if (neighbor.IsClear && neighbor.Disaster == null && neighbor.IsEdge == false)
                return true;
        return false;
    }
    public bool HasAdjacentOrthogonalClearSpace(int a_distance) {
        var neighbors = Board.instance.GetNeighborsOrthogonal(x, y, a_distance);
        foreach (var neighbor in neighbors)
            if (neighbor.IsClear && neighbor.Disaster == null && neighbor.IsEdge == false)
                return true;
        return false;
    }

    public Disaster Disaster {
        get => m_disaster;
        set {
            m_disaster = value;
            UpdateInfo();
            UpdateOverlay();
        }
    }

    public Player Controller {
        get => m_controller;
        set {
            m_controller = value;
            UpdateInfo();
            UpdateOverlay();
            if (m_controller == null)
                return;
            IsBlinking = true;
            AudioSource.PlayClipAtPoint(Board.instance.SoundPlaceStone, Camera.main.transform.position);
        }
    }

    public bool IsBlinking {
        get => m_isBlinking;
        set {
            m_blinkTimer = 0f;
            m_isBlinking = value;
        }
    }

    private bool m_isBlinking = false;

    private Player m_controller = null;
    private Disaster m_disaster = null;

    public void Clear() {
        m_controller = null;
        m_disaster = null;
        UpdateInfo();
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
        return Board.instance.HasNeighborOrthogonal(x, y, a_player.Color) && IsClear;
    }

    public void Click() {
        //GetComponent<Button>().Select();
        ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    private Direction m_direction = Direction.None;
    public Direction Direction {
        get => m_direction;
        set {
            m_direction = value;
            UpdateOverlay();
        }
    }

    public BoardTile NextTile {
        get {
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
    }

    private void Start() {
        UpdateInfo();

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            Board.instance.ActivateCard(this);
        });

        var sd = GetComponent<RectTransform>().sizeDelta;
        m_image.GetComponent<RectTransform>().sizeDelta = Board.instance.TileSize * Vector2.one;
        m_overlayImage.GetComponent<RectTransform>().sizeDelta = Board.instance.TileSize * Vector2.one;

        m_originalOverlayColor = m_overlayImage.color;
    }

    private float m_blinkTimer = 0f;
    private float m_blinkTimerTotal = 0f;
    private bool m_isOverlayVisible = true;
    private Color m_originalOverlayColor = Color.white;

    private void Update() {
        if (IsBlinking == false) {
            m_overlayImage.color = m_originalOverlayColor;
            return;
        }

        m_overlayImage.color = m_isOverlayVisible ? m_originalOverlayColor : Color.clear;
        m_blinkTimerTotal += Time.deltaTime;
        if (m_blinkTimerTotal > Board.instance.BlinkTimeTotal) {
            IsBlinking = false;
            return;
        }
        m_blinkTimer += Time.deltaTime;
        if (m_blinkTimer > Board.instance.BlinkFlipTime) {
            m_isOverlayVisible = !m_isOverlayVisible;
            m_blinkTimer = 0f;
        }
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
            sprite = (m_controller == null)
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
