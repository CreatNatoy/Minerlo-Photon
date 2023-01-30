using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;

    private GameObject[,] _cells;
    private List<PlayerController> _players = new List<PlayerController>();

    public void AddPlayer(PlayerController player)
    {
        _players.Add(player);
        
        _cells[player.GamePosition.x, player.GamePosition.y].SetActive(false);
    }

    private void Start()
    {
        _cells = new GameObject[20, 10];

        for (int x = 0; x < _cells.GetLength(0); x++)
        {
            for (int y = 0; y < _cells.GetLength(1); y++)
            {
                _cells[x, y] = Instantiate(_cellPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
    }
}
