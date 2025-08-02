using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public class NetworkStatsPP
{
    // 네트워크 통계 추적 변수들
    private float  ping                      = 0f;
    private float  lastPingTime              = 0f;
    private string serverIP                  = "";
    private System.Net.NetworkInformation.Ping pingSender;
    private bool   isPingInProgress          = false; // 핑 측정 중인지 확인
    private int    totalBytesReceived        = 0;
    private int    totalBytesSent            = 0;
    private int    packetsReceived           = 0;
    private int    packetsSent               = 0;
    
    // 패킷 로스 감지용 변수들 - 스레드 안전 버전
    private System.DateTime lastPacketReceiveTime     = System.DateTime.Now;
    private double          packetTimeoutThreshold    = 2.0; // 2초 이상 무응답시 로스로 간주
    private int             consecutiveTimeouts       = 0;
    private bool            isConnectionHealthy       = true;
    
    // 1초 단위 측정용 변수들
    private float  lastStatsTime             = 0f;
    private int    lastSecondBytesReceived   = 0;
    private int    lastSecondBytesSent       = 0;
    private int    lastSecondPacketsReceived = 0;
    private int    lastSecondPacketsSent     = 0;
    
    // 실시간 통계 변수들
    private float  downloadSpeed             = 0f; // KB/s
    private float  uploadSpeed               = 0f; // KB/s
    private int    downloadPacketsPerSecond  = 0;
    private int    uploadPacketsPerSecond    = 0;
    private float  downloadPacketLoss        = 0f; // %
    private float  uploadPacketLoss          = 0f; // %
    
    // MonoBehaviour 참조 (코루틴 실행용)
    private MonoBehaviour monoBehaviourRef;
    
    // 패킷 로스 추적용 - 성능 최적화된 카운터 방식
    private int    downloadSuccessCount     = 0;
    private int    downloadFailCount        = 0;
    private int    uploadSuccessCount       = 0;
    private int    uploadFailCount          = 0;
    private float  lastLossCalculationTime  = 0f;
    private const float LOSS_UPDATE_INTERVAL = 1.0f; // 1초마다만 로스율 계산
    
    public void Init()
    {
        // MonoBehaviour 참조 설정 (NetworkManager)
        monoBehaviourRef = NetworkManager.Instance;
        
        // 네트워크 통계 초기화
        lastStatsTime           = Time.time;
        lastPingTime            = Time.time;
        lastPacketReceiveTime   = System.DateTime.Now; // 스레드 안전
        lastLossCalculationTime = Time.time;
        
        // 핑 객체 초기화
        try
        {
            pingSender = new System.Net.NetworkInformation.Ping();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Ping initialization failed: {e.Message}");
        }
    }
    
    public void Clear()
    {
        // 핑 객체 정리
        if (pingSender != null)
        {
            pingSender.Dispose();
            pingSender = null;
        }
        
        // 통계 초기화
        ping                      = 0f;
        totalBytesReceived        = 0;
        totalBytesSent            = 0;
        packetsReceived           = 0;
        packetsSent               = 0;
        lastSecondBytesReceived   = 0;
        lastSecondBytesSent       = 0;
        lastSecondPacketsReceived = 0;
        lastSecondPacketsSent     = 0;
        downloadSpeed             = 0f;
        uploadSpeed               = 0f;
        downloadPacketsPerSecond  = 0;
        uploadPacketsPerSecond    = 0;
        downloadPacketLoss        = 0f;
        uploadPacketLoss          = 0f;
        isPingInProgress          = false;
        
        // 패킷 로스 추적 초기화 - 성능 최적화
        downloadSuccessCount      = 0;
        downloadFailCount         = 0;
        uploadSuccessCount        = 0;
        uploadFailCount           = 0;
        lastPacketReceiveTime     = System.DateTime.Now; // 스레드 안전
        lastLossCalculationTime   = 0f;
        consecutiveTimeouts       = 0;
        isConnectionHealthy       = true;
    }
    
    // 서버 IP 설정 (IPv4 주소 검증 포함)
    public void SetServerIP(string ip)
    {
        // IPv4 주소인지 확인하고 설정
        if (System.Net.IPAddress.TryParse(ip, out System.Net.IPAddress address))
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                serverIP = ip;
                Debug.Log($"Network Stats: Using IPv4 address {ip} for ping measurement");
            }
            else
            {
                // IPv6 주소인 경우 로컬호스트로 대체
                serverIP = "127.0.0.1";
                Debug.LogWarning($"Network Stats: IPv6 address detected ({ip}), using localhost (127.0.0.1) instead");
            }
        }
        else
        {
            // 파싱 실패시 로컬호스트 사용
            serverIP = "127.0.0.1";
            Debug.LogWarning($"Network Stats: Invalid IP address ({ip}), using localhost (127.0.0.1) instead");
        }
    }
    
    // 수신 패킷 통계 업데이트 - 성능 최적화
    public void OnPacketReceived(int packetSize)
    {
        packetsReceived++;
        totalBytesReceived += packetSize;
        
        // 패킷 수신 시간 업데이트 (스레드 안전한 방식)
        lastPacketReceiveTime = DateTime.Now;
        consecutiveTimeouts = 0; // 타임아웃 카운터 리셋
        isConnectionHealthy = true;
        
        // 성공적인 패킷 수신 카운터 증가 (O(1) 연산)
        downloadSuccessCount++;
    }
    
    // 송신 패킷 통계 업데이트 - 성능 최적화  
    public void OnPacketSent(int packetSize)
    {
        packetsSent++;
        totalBytesSent += packetSize;
        
        // 성공적인 패킷 송신 카운터 증가 (O(1) 연산)
        uploadSuccessCount++;
    }
    
    // 메인 업데이트 (NetworkManager.Update에서 호출)
    public void UpdateStats()
    {
        if (!NetworkManager.Instance.isNetworkInit_Complete) return;
        
        // 비동기 핑 측정 시작 (블로킹 방지)
        if (Time.time - lastPingTime > 1.0f && pingSender != null && !isPingInProgress)
        {
            if (monoBehaviourRef != null)
            {
                monoBehaviourRef.StartCoroutine(MeasurePingAsync());
            }
            lastPingTime = Time.time;
        }
        
        // 연결 상태 모니터링 비활성화 (기존 기능 복구)
        // CheckConnectionHealth(); // 임시 비활성화
        
        // 1초마다 통계 업데이트
        if (Time.time - lastStatsTime >= 1.0f)
        {
            float deltaTime = Time.time - lastStatsTime;
            
            // 속도 계산 (KB/s)
            downloadSpeed = (totalBytesReceived - lastSecondBytesReceived) / 1024f / deltaTime;
            uploadSpeed   = (totalBytesSent     - lastSecondBytesSent)     / 1024f / deltaTime;
            
            // 패킷 수 계산
            downloadPacketsPerSecond = Mathf.RoundToInt((packetsReceived - lastSecondPacketsReceived) / deltaTime);
            uploadPacketsPerSecond   = Mathf.RoundToInt((packetsSent     - lastSecondPacketsSent)     / deltaTime);
            
            // 이전 값 저장
            lastSecondBytesReceived   = totalBytesReceived;
            lastSecondBytesSent       = totalBytesSent;
            lastSecondPacketsReceived = packetsReceived;
            lastSecondPacketsSent     = packetsSent;
            lastStatsTime             = Time.time;
            
            // 패킷 로스율 계산 (1초마다만 - 성능 최적화)
            if (Time.time - lastLossCalculationTime >= LOSS_UPDATE_INTERVAL)
            {
                UpdatePacketLossStats();
                lastLossCalculationTime = Time.time;
            }
            
            // UI 업데이트
            UpdateNetworkStatsUI();
        }
    }
    
    // 비동기 핑 측정 코루틴 (Update 블로킹 방지)
    private IEnumerator MeasurePingAsync()
    {
        isPingInProgress = true;
        
        // 별도 스레드에서 핑 측정
        PingReply reply    = null;
        bool      pingCompleted = false;
        
        // 백그라운드에서 핑 실행
        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                reply         = pingSender.Send(serverIP, 1000); // 1초 타임아웃
                pingCompleted = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Ping measurement failed: {e.Message}");
                pingCompleted = true;
            }
        });
        
        // 핑 완료 대기 (최대 1.5초)
        float timeout = 1.5f;
        while (!pingCompleted && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
        
        // 결과 처리
        if (pingCompleted && reply != null && reply.Status == IPStatus.Success)
        {
            ping = reply.RoundtripTime;
        }
        else
        {
            ping = 999f; // 실패시 높은 값
        }
        
        isPingInProgress = false;
    }
    
    // 네트워크 통계 UI 업데이트
    private void UpdateNetworkStatsUI()
    {
        if (!NetworkManager.Instance.isNetworkInit_Complete) return;
        
        try
        {
            if (ClientManager.UI?.manageUI?.PktInfoText != null)
            {
                // 고정 폭으로 글자 움직임 방지
                string pingSection = $"Ping {ping:F0}ms";
                string downSection = $"Down {downloadSpeed:F2}KB/s {downloadPacketsPerSecond}pkt/s {downloadPacketLoss:F1}% pkt loss";
                string upSection   = $"Up {uploadSpeed:F2}KB/s {uploadPacketsPerSecond}pkt/s {uploadPacketLoss:F1}% pkt loss";
                
                string statsText = $"{pingSection,-20}     {downSection,-50}     {upSection}";
                
                ClientManager.UI.manageUI.PktInfoText.text = statsText;
            }
        }
        catch (System.Exception e)
        {
            // UI 참조 에러 방지
            Debug.LogWarning($"Network stats UI update failed: {e.Message}");
        }
    }
    
    // 연결 상태 확인 및 패킷 로스 감지
    private void CheckConnectionHealth()
    {
        // 패킷 수신 타임아웃 체크 (스레드 안전 버전)
        if ((System.DateTime.Now - lastPacketReceiveTime).TotalSeconds > packetTimeoutThreshold)
        {
            // 타임아웃 발생
            consecutiveTimeouts++;
            isConnectionHealthy = false;
            
            // 패킷 로스 카운터 증가 (O(1) 연산)
            downloadFailCount++;
                
            // 심각한 연결 문제 감지
            if (consecutiveTimeouts >= 3)
            {
                Debug.LogWarning($"Network Health: Consecutive timeouts detected ({consecutiveTimeouts})");
            }
            
            // 타임아웃 시간 갱신 (무한 로그 방지)
            lastPacketReceiveTime = System.DateTime.Now;
        }
    }
    
    // 패킷 로스 통계 계산 - 성능 최적화 (O(1) 연산)
    private void UpdatePacketLossStats()
    {
        // 다운로드 패킷 로스 계산
        int totalDownloadPackets = downloadSuccessCount + downloadFailCount;
        if (totalDownloadPackets > 0)
        {
            downloadPacketLoss = (downloadFailCount / (float)totalDownloadPackets) * 100f;
        }
        else
        {
            downloadPacketLoss = 0f;
        }
        
        // 업로드 패킷 로스 계산
        int totalUploadPackets = uploadSuccessCount + uploadFailCount;
        if (totalUploadPackets > 0)
        {
            uploadPacketLoss = (uploadFailCount / (float)totalUploadPackets) * 100f;
        }
        else
        {
            uploadPacketLoss = 0f;
        }
        
        // 카운터 리셋 (슬라이딩 윈도우 효과)
        downloadSuccessCount = 0;
        downloadFailCount = 0;
        uploadSuccessCount = 0;
        uploadFailCount = 0;
    }
    
    // 수동으로 패킷 로스 기록 - 성능 최적화 (외부에서 호출 가능)
    public void RecordPacketLoss(bool isDownload)
    {
        if (isDownload)
        {
            downloadFailCount++;
        }
        else
        {
            uploadFailCount++;
        }
    }
    
    // 디버그용 정보 출력
    public string GetDebugInfo()
    {
        return $"Ping: {ping:F0}ms | Down: {downloadSpeed:F1}KB/s {downloadPacketsPerSecond}pkt/s {downloadPacketLoss:F1}% | Up: {uploadSpeed:F1}KB/s {uploadPacketsPerSecond}pkt/s {uploadPacketLoss:F1}% | Healthy: {isConnectionHealthy}";
    }
} 