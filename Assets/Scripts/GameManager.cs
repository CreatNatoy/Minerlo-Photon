using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _leaveRoom;

    private void Start()
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        
        _leaveRoom.onClick.AddListener(Leave);
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
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}