using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class Addressable_Test1 : MonoBehaviour
{
    public AssetReference[] obs; // 게임오브젝트들
    public AssetReference[] musics; // 음악

    public AudioSource audioSource; // 오디오소스

    public GameObject StartPanel; //UI
    public GameObject DownPanel; //UI
    public GameObject LoadingPanel; // UI

    private AsyncOperationHandle handle;


    public TextMeshProUGUI byteText; //다운로드 사이즈
    public Slider slider; // 로딩%
    public TextMeshProUGUI perText; //퍼센트 Text 

    // private void Start()
    // {
    //     Caching.ClearCache();
    // }

    public void StartBtn()
        //시작버튼 누름
    {
        StartCoroutine(StartFunc());
        StartPanel.SetActive(false);
    }

    IEnumerator StartFunc()
    {
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync("Test");
        //Test 이라는 라벨 다운로드사이즈 확인
        yield return getDownloadSize;

        if (getDownloadSize.Result > 0)
            //다운로드 사이즈가 0 이상이라면 다운받아야하기 때문에 다운로드UI 표시
        {
            DownPanel.SetActive(true);
            byteText.text = $"{getDownloadSize.Result} Byte";
        }
        else
        {
            //아니라면 다음으로 넘어간다.
            NextShow();
        }

        //메모리 내림
        Addressables.Release(getDownloadSize);
    }

    public void DownBtn()
        //다운로드 시작 버튼 누름
    {
        StartCoroutine(DownFunc());
        DownPanel.SetActive(false);
    }

    IEnumerator DownFunc()
    {
        handle = Addressables.DownloadDependenciesAsync("Test");
        //Test 라벨을 다운로드 받습니다.

        StartCoroutine(Show());
        //로딩바 보여줌
        yield return handle;
        yield return new WaitForSeconds(1);
        //1초대기

        LoadingPanel.SetActive(false);
        NextShow();
        //다음으로 넘어감

        //메모리 해제
        Addressables.Release(handle);
    }


    public void NextShow()
    {
        Addressables.InstantiateAsync(obs[0], new Vector3(-5, 0, 0), quaternion.identity);
        Addressables.InstantiateAsync(obs[1], new Vector3(-2.5f, 0, 0), quaternion.identity);
        Addressables.InstantiateAsync(obs[2], new Vector3(0, 0, 0), quaternion.identity);
        Addressables.InstantiateAsync(obs[3], new Vector3(2.5f, 0, 0), quaternion.identity);
        Addressables.InstantiateAsync(obs[4], new Vector3(5, 0, 0), quaternion.identity);
        //오브젝트 소환


        int ran = Random.Range(0, musics.Length);
        //랜덤으로 뮤직 하나 선택

        Addressables.LoadAssetAsync<AudioClip>(musics[ran]).Completed += (op) =>
        {
            if (op.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            audioSource.clip = op.Result;
            audioSource.Play();
            //뮤직 스타트
        };
    }

    IEnumerator Show()
        //다운로드 % 보여주는 함수
    {
        LoadingPanel.SetActive(true);
        yield return new WaitUntil(() => handle.IsValid());
        while (handle.PercentComplete < 1)
        {
            slider.value = handle.PercentComplete;
            perText.text = $"{handle.PercentComplete * 100:F2}%";
            yield return null;
        }

        slider.value = 1;
        perText.text = "100%";
    }
}