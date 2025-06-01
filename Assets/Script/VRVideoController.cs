using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

[RequireComponent(typeof(MeshFilter))]
public class VRVideoController : MonoBehaviourPunCallbacks
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public TextMeshProUGUI timeText;

    // private AudioSource audioSource;

    public string videoFileName = "aespa_Comp_v7_BMB.mp4";

    private bool isPrepared = false;
    private bool isPlaying = false;
    private double startTime;

    void Start()
    {
       
        // audioSource = GetComponent<AudioSource>();
        // if (audioSource == null)
        //     audioSource = gameObject.AddComponent<AudioSource>();

        // videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        // videoPlayer.EnableAudioTrack(0, true); 
        // videoPlayer.SetTargetAudioSource(0, audioSource);

        InvertMesh();

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = videoPath;

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();

        PhotonNetwork.ConnectUsingSettings();
    }

    void InvertMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        Mesh mesh = mf.mesh;
Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int temp = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = temp;
            }
            mesh.SetTriangles(triangles, i);
        }
    }

    void OnPrepared(VideoPlayer source)
    {
        isPrepared = true;
        Debug.Log("isPrepared Ready");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Connected");
        PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions { MaxPlayers = 3 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room. Am IMaster? " + PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        else
            playButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlaying && videoPlayer.isPlaying)
        {
            timeText.text = "Time: " + videoPlayer.time.ToString("F2") + "s";
        }
    }

    void OnPlayButtonClicked()
    {
        if(isPrepared)
        {
            startTime = PhotonNetwork.Time + 1.0f;
            photonView.RPC("StartVideo", RpcTarget.All, startTime);  
        }
    }

    [PunRPC]
    void StartVideo(double netTime)
    {
        startTime = netTime;
        float delay = (float)(startTime - PhotonNetwork.Time);

        if (delay > 0)
            Invoke(nameof(PlayVideo), delay);
        else
            PlayVideo();
    }
    void PlayVideo()
    {
        isPlaying = true;

        double elapsed = PhotonNetwork.Time - startTime;
        if (elapsed < 0) elapsed = 0;

        videoPlayer.time = elapsed;
        Debug.Log($"디버깅 시간 {elapsed * 1000.0} ms");

        videoPlayer.Play();
    }
}
