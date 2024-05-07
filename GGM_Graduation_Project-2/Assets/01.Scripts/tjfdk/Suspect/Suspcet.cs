using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspcet : MonoBehaviour
{
    [SerializeField] private Sprite currentFace => CurrentFace;
    [SerializeField] private Sprite[] faces => Faces;
    [SerializeField] private string name => Name;
    [SerializeField] private string[] feature => Feature;
    [SerializeField] private string memo => Memo;
    [SerializeField] private int doubtLevel => DoubtLevel;
    [SerializeField] private int doubtLevelMax => DoubtLevelMax;

    public Sprite CurrentFace;
    public Sprite[] Faces;
    public string Name;
    public string[] Feature;
    public string Memo;
    public int DoubtLevel;
    public int DoubtLevelMax;

    private void Awake()
    {
        CurrentFace = faces[0];
    }
}
