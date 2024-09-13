using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private float scrollerSpeed;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float creditsLimit;

    [SerializeField] private Image titleImg;
    [SerializeField] private Image logoImg;
    [SerializeField] private TextMeshProUGUI logoText;

    [SerializeField] private GameObject credits;

    private RectTransform creditsRectTransform;
    private bool isFade = false;
    private bool isCreditsStopped = false;

    private void Start()
    {
        creditsRectTransform = credits.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!isCreditsStopped)
        {
            if (isFade)
            {
                creditsRectTransform.anchoredPosition += Vector2.up * scrollerSpeed * Time.deltaTime;

                if (creditsRectTransform.anchoredPosition.y >= creditsLimit)
                {
                    Debug.Log("Credits stopped");
                    scrollerSpeed = 0;
                    isCreditsStopped = true;
                    StartCoroutine(FadeOutLogo());
                }
            }
            else
            {
                StartCoroutine(FadeInTitle());
            }
        }
    }

    private IEnumerator FadeInTitle()
    {
        Color titleColor = titleImg.color;
        while (titleImg.color.a < 1f)
        {
            titleColor.a += fadeSpeed * Time.deltaTime;
            titleImg.color = titleColor;
            yield return null;
        }
        isFade = true;
    }

    private IEnumerator FadeOutLogo()
    {
        Color logoColor = logoImg.color;
        while (logoImg.color.a > 0f)
        {
            logoColor.a -= 2 * Time.deltaTime;
            logoImg.color = logoColor;
            logoText.color = logoColor;
            yield return null;
        }
    }

    public void GameBtn()
    {
        SceneManager.LoadScene(0);
        SoundManager.Instance.StopBGM();
    }
}
