using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    public CanvasGroup canvasGroup;
    public GameObject[] loadingImage;
    private bool is_Loading;
    
    private WaitForSeconds dotWait = new WaitForSeconds(0.5f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadingStart();
    }

    private IEnumerator LoadingImage()
    {
        while (is_Loading)
        {
            for (int i = 0; i < loadingImage.Length; i++)
            {
                loadingImage[i].gameObject.SetActive(true);
                yield return dotWait;
            }
            for (int i = 0; i < loadingImage.Length; i++)
            {
                loadingImage[i].gameObject.SetActive(false);
            }
            yield return dotWait;
        }
    }

    public void LoadingStart()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.DOFade(1, 1f);
        is_Loading = true;

        StartCoroutine(LoadingImage());

        // 무언가 있다면 더 해주기
    }

    public void LoadingEnd()
    {
        is_Loading = false;

        StopCoroutine(LoadingImage());

        for (int i = 0; i < loadingImage.Length; i++)
        {
            loadingImage[i].SetActive(false);
        }
        canvasGroup.gameObject.SetActive(false);

        canvasGroup.DOFade(0, 1f);
    }

}