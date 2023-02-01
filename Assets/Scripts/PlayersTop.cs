using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayersTop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _texts; 
    
    private void Start() {
        foreach (var text in _texts) {
            text.text = ""; 
        }
    }

    public void SetText(List<PlayerController> players) {
        PlayerController[] top = players
            .Where(p => !p.IsDead)
            .OrderByDescending(p => p.Score)
            .Take(5)
            .ToArray();

        for (int i = 0; i < top.Length; i++) {
            _texts[i].text = $"{i + 1}. {top[i].PhotonView.Owner.NickName}   {top[i].Score}";
        }
        
    }
}
