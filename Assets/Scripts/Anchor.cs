using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor {
    public Vector3 position;

    public Anchor() {
        World.instance.anchors.Add(this);
    }
}
