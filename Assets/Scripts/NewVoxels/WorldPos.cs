using UnityEngine;
using System.Collections;
using System;
using MessagePack;

[MessagePackObject]
public struct WorldPos : IEquatable<WorldPos>
{
    [Key(0)]
    public int x;
    [Key(1)]
    public int y;
    [Key(2)]
    public int z;

    public WorldPos(Vector3 pos)
    {
        this.x = (int)pos.x;
        this.y = (int)pos.y;
        this.z = (int)pos.z;
    }

    public WorldPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static int GenerateHashCode(int x, int y, int z) {
        unchecked
        {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            hash = hash * 227 + z.GetHashCode();

            return hash;
        }
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 227 + y.GetHashCode();
            hash = hash * 227 + z.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (GetHashCode() == obj.GetHashCode())
            return true;
        return false;
    }

    public static bool operator ==(WorldPos v0, WorldPos v1) => v0.Equals(v1);
    public static bool operator !=(WorldPos v0, WorldPos v1) => !v0.Equals(v1);

    public bool Equals(WorldPos other) => x == other.x && y == other.y && z == other.z;

    public static WorldPos operator+(WorldPos pos1, WorldPos pos2) {
        return new WorldPos(pos1.x + pos2.x, pos1.y + pos2.y, pos1.z + pos2.z);
    }
    public static WorldPos operator *(WorldPos pos1, int constant)
    {
        return new WorldPos(pos1.x * constant, pos1.y * constant, pos1.z * constant);
    }

    public Vector3 ToVector3() {
        Vector3 vec3 = new Vector3(x, y, z);
        return vec3;
    }

    public override string ToString()
    {
        return "[" + x + ", " + y + ", " + z + "]";
    }
}