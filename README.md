# 사전과제 프로젝트: Photon 기반 멀티유저 비디오 동기화 시스템

## 개요  
본 프로젝트는 Unity와 Photon PUN(Photon Unity Networking)을 활용하여 다수의 유저가 동일한 비디오를 네트워크 상에서 실시간으로 동기화하여 재생할 수 있는 시스템을 구현하였습니다.  

---

## 주요 기능

### 1. Photon PUN을 이용한 멀티유저 연결 및 방 관리  
- `PhotonNetwork.ConnectUsingSettings()`로 마스터 서버에 연결  
- `JoinOrCreateRoom`을 통해 방 자동 생성 및 입장  
- 마스터 클라이언트가 재생 버튼 활성화, 게스트는 버튼 비활성화 및 숨김 처리

### 2. `IsMasterClient` 활용한 UI 분기  
- 메인 클라이언트(호스트)만 재생 제어 버튼을 표시하고 활성화  
- 게스트 클라이언트는 재생 버튼을 비활성화하여 조작 불가능하게 함  
- UI를 메인용과 게스트용으로 분리하여 각각 다른 화면 표시 가능

### 3. 동기화된 비디오 재생 제어: `StartVideo` RPC 함수

```csharp
[PunRPC]
void StartVideo(double networkStartTime)
{
    startTime = networkStartTime;
    Invoke(nameof(PlayVideo), (float)(startTime - PhotonNetwork.Time));
}
```
- 이 함수는 네트워크 시간을 기준으로 모든 클라이언트가 동시에 비디오를 재생하도록 동기화하는 역할을 합니다.
- networkStartTime은 Photon 네트워크 시간을 기준으로 미리 정해진 재생 시작 시점입니다.
- Invoke 메서드는 현재 클라이언트의 PhotonNetwork.Time과 비교해 정확한 재생 딜레이를 계산하여, 각 클라이언트가 동일한 시점에 PlayVideo()를 호출하게 만듭니다.
- 이를 통해 네트워크 지연이나 각 클라이언트 시간차에도 불구하고 모든 유저가 같은 타이밍에 비디오를 시작할 수 있도록 보장합니다.

## 개발 환경 및 빌드

- Unity 6000.0.45f1 LTS
- Unity Asset -> Photon PUN 2 최신 버전
- 타겟 플랫폼: macOS


## 시연 방법

[![Video Label](https://velog.velcdn.com/images/yujinaa/post/860ecb57-4984-4912-8b8b-e3f16b9e73f9/image.png)](https://youtu.be/AeQYozCcH7M)
*클릭 시 유튜브로 이동합니다. 현재 본가에 있어 주변 소음이 있을 수 있으니 양해 부탁드립니다.*

빌드 및 실행 후, 빌드된 프로그램 인스턴스와 유니티 에디터 시뮬레이터 두 대로 실행.
- 한쪽은 자동으로 마스터 클라이언트가 되어 main으로서 재생 버튼을 조작 가능.
- 한쪽이 마스터 클라이언트가 된 이후, 접속된 클라이언트는 자동으로 게스트 클라이언트가 됨.
- 버튼 클릭 시 네트워크 시간 기준으로 모든 클라이언트의 비디오가 동기화되어 재생됨.    
+ 마스터 클라이언트와 게스트 클라이언트 UI가 각각 분리되어 사용자 구분.


## 느낀 점 및 한계

- Photon의 네트워크 시간 기능과 RPC 호출을 적절히 활용하면 멀티 유저 동기화 작업이 비교적 수월함을 경험.
- UI 역할 분기와 네트워크 동기화 로직의 기본을 익힐 수 있었음.
- 약간의 지연은 있으나, 정밀 동기화 가능성 확인함.

