using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _leaveRoom;
    [SerializeField] private MapController _mapController;

    private void Start()
    {
        Vector3 position = new Vector3(Random.Range(1, 15), Random.Range(1, 5));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        
        _leaveRoom.onClick.AddListener(Leave);

        PhotonPeer.RegisterType(typeof(Vector2Int), 242, SerializeVector2Int, DeserializeVector2Int); 
        PhotonPeer.RegisterType(typeof(SyncData), 243, SyncData.Serialize, SyncData.Deserialize); 
    }

    private void OnDestroy()
    {
        _leaveRoom.onClick.RemoveAllListeners();
    }

    private void Leave()
    {
        PhotonNetwork.LeaveRoom(); 
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0); 
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient) {
            _mapController.SendSyncData(newPlayer);
        }
        
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
     //   PlayerController player = _mapController.Players.First(p => p.PhotonView.Owner == null);
        PlayerController player = _mapController.Players.First(p => p.PhotonView.CreatorActorNr == otherPlayer.ActorNumber);
        if(player != null) player.Kill();
        
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    public static object DeserializeVector2Int(byte[] data)
    {
        Vector2Int result = new Vector2Int();

        result.x = BitConverter.ToInt32(data, 0);
        result.y = BitConverter.ToInt32(data, 4);

        return result;
    }

    public static byte[] SerializeVector2Int(object obj)
    {
        Vector2Int vector = (Vector2Int)obj;
        byte[] result = new byte[8];

        BitConverter.GetBytes(vector.x).CopyTo(result, 0);
        BitConverter.GetBytes(vector.y).CopyTo(result, 4);

        return result; 
    }
}