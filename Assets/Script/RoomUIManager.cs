using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public GameObject mainUI;
    public GameObject guestUI;
    public Button playButton;

    public override void OnJoinedRoom()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;

        mainUI.SetActive(isMaster);
        playButton.gameObject.SetActive(isMaster);

        guestUI.SetActive(!isMaster);
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
    }
}