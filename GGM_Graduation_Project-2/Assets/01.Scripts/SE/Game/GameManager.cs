using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Canvas;
    private Image panel;

    private void Awake()
    {
        Instance = this;
        panel = Canvas.GetComponentInChildren<Image>();
    }

    public void DemoEnd()
    {
        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        Canvas.gameObject.SetActive(true);
        panel.DOFade(1, 2);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
