using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _logText;

    [Header("Button")] 
    [SerializeField] private Button _createRoom; 
    [SerializeField] private Button _joinRandomRoom; 

    private void Start()
    {
        PhotonNetwork.NickName = "Player" + Random.Range(1000, 10000);
        Log("Player's name is set to " + PhotonNetwork.NickName);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();

        _createRoom.onClick.AddListener(CreateRoom);
        _joinRandomRoom.onClick.AddListener(JoinRoom);
    }

    public override void OnConnectedToMaster()
    {
      Log("Connected to Master");  
    }

    private void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Log("Joined the room");
        
        PhotonNetwork.LoadLevel("Game");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        _logText.text += "\n";
        _logText.text += message; 
    }
}
