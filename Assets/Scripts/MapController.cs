using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MapController : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private GameObject _cellPrefab;

    private GameObject[,] _cells;
    private List<PlayerController> _players = new List<PlayerController>();

    private double _lastTickTime;

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

    private void Update() {
        if (PhotonNetwork.Time > _lastTickTime + 1 &&
            PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == 2) {

            Vector2Int[] directions = _players
                .OrderBy(p => p.PhotonView.Owner.ActorNumber)
                .Select(p => p.Direction)
                .ToArray();
            
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(42, directions, options, sendOptions); 
            
            PerformTick(directions);
        }
    }

    public void OnEvent(EventData photonEvent) {
        switch (photonEvent.Code) {
            case 42:
                Vector2Int[] directions = (Vector2Int[]) photonEvent.CustomData;

                PerformTick(directions);
                
                break;
        }
    }

    private void PerformTick(Vector2Int[] directions) {
        if(_players.Count != directions.Length) return;

        var sortedPlayer = _players.OrderBy(p => p.PhotonView.Owner.ActorNumber).ToArray();
        
        int i = 0;
        foreach (var player in sortedPlayer) {
            player.Direction = directions[i++];
            MinePlayerBlock(player);
        }
        
        foreach (var player in sortedPlayer) {
            MovePlayer(player);
        }

        _lastTickTime = PhotonNetwork.Time;
    }

    private void MinePlayerBlock(PlayerController player) {
       var targetPosition = player.GamePosition + player.Direction;

        if (targetPosition.x < 0) return;
        if (targetPosition.y < 0) return;
        if (targetPosition.x >= _cells.GetLength(0)) return;
        if (targetPosition.y >= _cells.GetLength(1)) return;

        _cells[player.GamePosition.x, player.GamePosition.y].SetActive(false);

        Vector2Int position = targetPosition;
        PlayerController minePlayer = _players.First(p => p.PhotonView.IsMine);
        if (minePlayer != player) {
            while (position.y < _cells.GetLength(1) && !_cells[position.x, position.y].activeSelf) {
                if (position == minePlayer.GamePosition) {
                    PhotonNetwork.LeaveRoom();
                    break;
                }
                position.y++; 
            }
        }
    }

    private void MovePlayer(PlayerController player) {
        player.GamePosition += player.Direction;

        if (player.GamePosition.x < 0) player.GamePosition.x = 0;
        if (player.GamePosition.y < 0) player.GamePosition.y = 0;
        if (player.GamePosition.x >= _cells.GetLength(0)) player.GamePosition.x = _cells.GetLength(0) - 1;
        if (player.GamePosition.y >= _cells.GetLength(1)) player.GamePosition.y = _cells.GetLength(1) - 1;
        
        int ladderLength = 0;
        Vector2Int position = player.GamePosition;
        while (position.y > 0 && !_cells[position.x, position.y - 1].activeSelf) {
            ladderLength++;
            position.y--;
        }

        player.SetLadderLength(ladderLength);
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
