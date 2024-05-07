using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    public abstract class sese
    {
        public abstract void s();
    }

    public class aa : sese
    {
        public int a = 1;

        public override void s()
        {
            Debug.Log("a");
        }
    }

    [SerializeField]
    public List<sese> seses = new List<sese>();

    private void Start()
    {
        aa bb = new aa();

        seses.Clear();
        seses.Add(bb);

        foreach (var item in seses)
        {
            Debug.Log(item);
        }
    }
}
