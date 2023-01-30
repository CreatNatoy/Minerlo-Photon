using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [Space] 
    [SerializeField] private PhotonView _photonView;

    private void Update()
    {
        if (!_photonView.IsMine) return; 
        
        if(Input.GetKey(KeyCode.LeftArrow)) transform.Translate(-Time.deltaTime * _speed, 0, 0);
        if(Input.GetKey(KeyCode.RightArrow)) transform.Translate(Time.deltaTime * _speed, 0, 0);
    }
}