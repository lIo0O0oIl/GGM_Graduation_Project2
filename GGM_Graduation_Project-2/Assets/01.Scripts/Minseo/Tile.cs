using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI numText;
    private Board board;
    private Vector3 correctPosition;

    public bool IsCorrected { private set; get; } = false;

    private int numeric;
    public int Numeric
    {
        set
        {
            numeric = value;
            numText.text = numeric.ToString();
        }
        get => numeric;
    }

    public void Setup(Board board, int hideNumeric, int numeric)
    {
        this.board = board;
        numText = GetComponentInChildren<TextMeshProUGUI>();

        Numeric = numeric;
        if (Numeric == hideNumeric)
        {
            GetComponent<UnityEngine.UI.Image>().enabled = false;
            numText.enabled = false;
        }
    }

    public void SetPosition()
    {
        correctPosition = GetComponent<RectTransform>().localPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        board.IsMoveTile(this);
    }

    public void OnMoveTo(Vector3 end)
    {
        StartCoroutine(MoveTo(end));
    }

    private IEnumerator MoveTo(Vector3 end)
    {
        float current = 0;
        float percent = 0;
        Vector3 start = GetComponent<RectTransform>().localPosition;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 0.1f;

            GetComponent<RectTransform>().localPosition = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        IsCorrected = correctPosition == GetComponent<RectTransform>().localPosition ? true : false;

        board.IsGameOver();
    }

}