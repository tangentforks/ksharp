using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp.Serialization
{
    /// <summary>
    /// Binary reader for K serialization format with little-endian encoding
    /// </summary>
    public class KSerializationReader
    {
        private readonly byte[] data;
        private int position = 0;
        
        public KSerializationReader(byte[] data)
        {
            this.data = data;
        }
        
        public int ReadInt32()
        {
            if (position + 4 > data.Length) throw new ArgumentException("Insufficient data for Int32");
            var bytes = data[position..(position + 4)];
            position += 4;
            
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            else
            {
                // Reverse for big-endian systems
                return BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
            }
        }
        
        public double ReadDouble()
        {
            if (position + 8 > data.Length) throw new ArgumentException("Insufficient data for Double");
            var bytes = data[position..(position + 8)];
            position += 8;
            
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToDouble(bytes, 0);
            }
            else
            {
                // Reverse for big-endian systems
                return BitConverter.ToDouble(bytes.Reverse().ToArray(), 0);
            }
        }
        
        public byte[] ReadBytes(int count)
        {
            if (position + count > data.Length) throw new ArgumentException("Insufficient data for bytes");
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
            if (position >= data.Length) throw new ArgumentException("Insufficient data for byte");
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
        
        public int Position 
        { 
            get => position; 
            set => position = value; 
        }
        
        public bool HasMoreData => position < data.Length;
    }
}
