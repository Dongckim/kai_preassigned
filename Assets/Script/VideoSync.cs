using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class VideoSync : MonoBehaviourPunCallbacks
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public TextMeshProUGUI timeText;

    private bool isPlaying = false;
    private double startTime;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, "yo.mp4");
        videoPlayer.url = videoPath;

        Debug.Log("Video URL: " + videoPath);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");

        RoomOptions options = new RoomOptions { MaxPlayers = 3 };
        PhotonNetwork.JoinOrCreateRoom("Room1", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room. Am I Master? " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        else
            playButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlaying)
        {
            timeText.text = "Time: " + videoPlayer.time.ToString("F2");
        }
    }

    void OnPlayButtonClicked()
    {
        startTime = PhotonNetwork.Time + 1.0f;
        photonView.RPC("StartVideo", RpcTarget.All, startTime);
    }

    [PunRPC]
    void StartVideo(double networkStartTime)
    {
        startTime = networkStartTime;
        Invoke(nameof(PlayVideo), (float)(startTime - PhotonNetwork.Time));
    }


    void PlayVideo()
    {
        isPlaying = true;
        videoPlayer.Play();
    }
}
