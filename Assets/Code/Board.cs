using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private int m_size = 6;
    [SerializeField] private BoardTile m_tilePrefab = null;

    private BoardTile[,] m_tileMap = null;

    private void Start() {
        m_tileMap = new BoardTile[m_size, m_size];
        CreateTileButtons();
        LinkTileButtons();
    }

    private void CreateTileButtons() {
        var pos = Vector2Int.zero;
        var tileRect = m_tilePrefab.GetComponent<RectTransform>().rect;
        var tileWidth = Mathf.FloorToInt(tileRect.width);
        var tileHeight = Mathf.FloorToInt(tileRect.height);
        for (var y = 0; y < m_size; ++y) {
            for (var x = 0; x < m_size; ++x) {
                m_tileMap[x, y] = Instantiate(m_tilePrefab, transform);
                var rectTransform = m_tileMap[x, y].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = pos;
                pos.x += tileHeight;
            }
            pos.x = 0;
            pos.y -= tileWidth;
        }
    }

    private void LinkTileButtons() {
        for (var y = 0; y < m_size; ++y) {
            for (var x = 0; x < m_size; ++x) {
                var button = m_tileMap[x, y].GetComponent<Button>();
            }
        }
    }
}
