using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp.Serialization
{
    /// <summary>
    /// Binary writer for K serialization format with little-endian encoding
    /// </summary>
    public class KBinaryWriter
    {
        private readonly List<byte> buffer = new List<byte>();
        
        public void WriteInt32(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                buffer.AddRange(bytes);
            }
            else
            {
                // Reverse for big-endian systems
                buffer.AddRange(bytes.Reverse());
            }
        }
        
        public void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                buffer.AddRange(bytes);
            }
            else
            {
                // Reverse for big-endian systems
                buffer.AddRange(bytes.Reverse());
            }
        }
        
        public void WriteBytes(byte[] value)
        {
            buffer.AddRange(value);
        }
        
        public void WriteString(string value)
        {
            buffer.AddRange(Encoding.UTF8.GetBytes(value));
        }
        
        public void WriteByte(byte value)
        {
            buffer.Add(value);
        }
        
        public void WriteInt16(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                buffer.AddRange(bytes);
            }
            else
            {
                // Reverse for big-endian systems
                buffer.AddRange(bytes.Reverse());
            }
        }
        
        public void WritePadding(int count)
        {
            buffer.AddRange(Enumerable.Repeat<byte>(0, count));
        }
        
        public byte[] ToArray()
        {
            return buffer.ToArray();
        }
        
        public int Length => buffer.Count;
    }
    
    /// <summary>
    /// Binary reader for K serialization format with little-endian encoding
    /// </summary>
    public class KBinaryReader
    {
        private readonly byte[] data;
        private int position = 0;
        
        public KBinaryReader(byte[] data)
        {
            this.data = data;
        }
        
        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(data, position);
            position += 4;
            return value;
        }
        
        public double ReadDouble()
        {
            var value = BitConverter.ToDouble(data, position);
            position += 8;
            return value;
        }
        
        public byte[] ReadBytes(int count)
        {
            var result = data[position..(position + count)];
            position += count;
            return result;
        }
        
        public string ReadString()
        {
            return ReadNullTerminatedString();
        }
        
        public byte ReadByte()
        {
            return data[position++];
        }
        
        private string ReadNullTerminatedString()
        {
            var start = position;
            while (position < data.Length && data[position] != 0)
            {
                position++;
            }
            return Encoding.UTF8.GetString(data, start, position - start);
        }
        
        public int Position => position;
        
        public bool HasMoreData => position < data.Length;
    }
}
