using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private int m_size = 6;
    [SerializeField] private BoardTile m_tilePrefab = null;

    private BoardTile[,] m_tileMap = null;
    private int m_tileSize = 0;

    public BoardTile GetTile(int a_tileX, int a_tileY) {
        return m_tileMap[a_tileX, a_tileY];
    }

    private void Awake() {
        var rect = m_tilePrefab.GetComponent<RectTransform>().rect;
        m_tileSize = Mathf.FloorToInt(rect.width);
    }

    private void Start() {
        m_tileMap = new BoardTile[m_size, m_size];
        CreateTileButtons();
        LinkTileButtons();
    }

    private void CreateTileButtons() {
        var pos = Vector2Int.zero;
        for (var y = 0; y < m_size; ++y) {
            for (var x = 0; x < m_size; ++x) {
                m_tileMap[x, y] = Instantiate(m_tilePrefab, transform);
                var rectTransform = m_tileMap[x, y].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = pos;
                pos.x += m_tileSize;
            }
            pos.x = 0;
            pos.y -= m_tileSize;
        }
    }

    private void LinkTileButtons() {
        for (var y = 0; y < m_size; ++y) {
            for (var x = 0; x < m_size; ++x) {
                var button = m_tileMap[x, y].GetComponent<Button>();
                var tileX = x;
                var tileY = y;
                button.onClick.AddListener(() => {
                    TurnManager.instance.ClickTile(tileX, tileY);
                });
            }
        }
    }
}
