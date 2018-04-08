using UnityEngine;
using System.Collections;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


//[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class BlockInstanceData : IEquatable<BlockInstanceData>
{
    public static readonly ushort TypeMask = 0x7FFF;

    /* Bits
     * 15 - solid
     * 14 - 0 - block type
    */
    public readonly ushort m_data;

    public BlockInstanceData(ushort data)
    {
        m_data = data;
    }

    public BlockInstanceData(ushort type, bool solid)
    {
        m_data = (ushort)(type & 0x7FFF);
        if (solid)
            m_data |= 0x8000;
    }

    /// <summary>
    /// Fast lookup of whether the block is solid without having to take a look into block arrays
    /// </summary>
    [MethodImpl(256)]
    public bool Solid()
    {
        return (m_data >> 15) != 0;
    }

    /// <summary>
    /// Information about block's type
    /// </summary>
    [MethodImpl(256)]
    public ushort Type()
    {
        return (ushort)(m_data & TypeMask);
    }

    public static ushort RestoreBlockInstanceData(byte[] data, int offset)
    {
        return BitConverter.ToUInt16(data, offset);
    }

    public static byte[] ToByteArray(BlockInstanceData data)
    {
        return BitConverter.GetBytes(data.m_data);
    }

    #region Object comparison

    public bool Equals(BlockInstanceData other)
    {
        return m_data == other.m_data;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        return obj is BlockInstanceData && Equals((BlockInstanceData)obj);
    }

    public override int GetHashCode()
    {
        return m_data.GetHashCode();
    }

    public static bool operator ==(BlockInstanceData data1, BlockInstanceData data2)
    {
        return data1.m_data == data2.m_data;
    }

    public static bool operator !=(BlockInstanceData data1, BlockInstanceData data2)
    {
        return data1.m_data != data2.m_data;
    }

    #endregion
}