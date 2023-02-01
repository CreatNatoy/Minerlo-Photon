using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _logText;

    [Space] 
    [SerializeField] private TMP_InputField _inputNickname;
    
    [Header("Button")] 
    [SerializeField] private Button _createRoom; 
    [SerializeField] private Button _joinRandomRoom; 

    private void Start() {
        string nickName = PlayerPrefs.GetString("NickName", "Player" + Random.Range(1000, 9999));
        PhotonNetwork.NickName = nickName;
        _inputNickname.text = nickName;
        Log("Player's name is set to " + nickName);

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

    private void CreateRoom() {
        SetName();
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2, CleanupCacheOnLeave = false});
    }

    private void SetName() {
        PhotonNetwork.NickName = _inputNickname.text;
        PlayerPrefs.SetString("NickName", _inputNickname.text);
    }

    private void JoinRoom()
    {
        SetName();
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
