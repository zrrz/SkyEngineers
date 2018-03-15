using UnityEngine;
using System.Collections;
using System;
using MessagePack;

[MessagePackObject]
public class BlockInstanceData
{
    [Key(0)]
    public int ID = 0; //ID of corresponding BlockData this is an instance of
    [IgnoreMember]
    public bool changed = true;

    //Base block constructor
    public BlockInstanceData()
    {

    }



}