using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor {
    public Vector3 position = Vector3.zero;

    public Anchor() {
        World.instance.anchors.Add(this);
    }

    ~Anchor() {
        World.instance.anchors.Remove(this);
    }
}
