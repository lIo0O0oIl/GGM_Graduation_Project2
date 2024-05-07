using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Evidence : MonoBehaviour
{
    [SerializeField] private Sprite image => Image;
    [SerializeField] private string who => Who;
    [SerializeField] private string name => Name;
    [SerializeField] private string feature => Feature;

    public Sprite Image;
    public string Who;
    public string Name;
    public string Feature;
}
