using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public List<Parent> parent = new List<Parent>();

    public Child child;
    public Chidl2 child2;

    private void Start()
    {
        parent.Add(child);
        parent.Add(child2);

        Child c = parent[0] as Child;
        if (c != null)
            Debug.Log(c.id);

        Chidl2 cc = parent[1] as Chidl2;
        if (cc != null)
            Debug.Log(cc.pi);

        Child a = parent[1] as Child;
        if (a != null)
            Debug.Log(a.id);

        Chidl2 aa = parent[0] as Chidl2;
        if (aa != null)
            Debug.Log(aa.pi);
    }

}
