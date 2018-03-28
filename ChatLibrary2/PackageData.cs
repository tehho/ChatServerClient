using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class PackageData
    {
        private readonly List<byte> _data;

        public byte[] Data => _data.ToArray();

        public int Position { get; set; }

        public int Size => _data.Count;

        public bool EOF => Position >= Size;

        public static int BufferSize = 4096;

        public PackageData()
        {
            _data = new List<byte>();
            Position = 0;
        }
        public PackageData(IEnumerable<byte> data)
        {
            _data = data.ToList();
            Position = 0;
        }
        public PackageData(IEnumerable<byte> data, int size)
        {
            _data = data.Take(size).ToList();
            Position = 0;
        }

        public void Reset() => Position = 0;

        public void Add(byte data)
        {
            _data.Add(data);
        }
        public void Add(IEnumerable<byte> data)
        {
            data.ToList().ForEach(Add);
        }
        public void Add(IEnumerable<byte> data, int size)
        {
            var temp = data.ToArray();
            for (int i = 0; i < size; i++)
            {
                Add(temp[i]);
            }
        }

        public byte GetData()
        {
            if (Position >= Size)
                throw new IndexOutOfRangeException("End of stream");
            byte data = _data[Position];
            Position++;

            return data;
        }
        public byte[] GetData(int size)
        {
            var list = new byte[size];

            if (Position + size > Size)
            {
                throw new IndexOutOfRangeException("End of stream");
            }

            Array.Copy(_data.ToArray(), Position, list, 0, size);

            Position += size;

            return list;
        }

        public void Add(int data)
        {
            Add(BitConverter.GetBytes(data));
        }
        public int GetInt()
        {
            return BitConverter.ToInt32(GetData(sizeof(int)), 0);
        }

        public void Add(short data)
        {
            Add(BitConverter.GetBytes(data));
        }
        public short GetShort()
        {
            return BitConverter.ToInt16(GetData(sizeof(short)), 0);
        }

        public void Add(char data)
        {
            Add(BitConverter.GetBytes(data));
        }
        public char GetChar()
        {
            return BitConverter.ToChar(GetData(sizeof(char)), 0);
        }

        public void Add(string data)
        {
            Add(data.Length);
            data.ToCharArray().ToList().ForEach(Add);
        }
        public string GetString()
        {
            int size = GetInt();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                sb.Append(GetChar());
            }
            return sb.ToString();
        }
    }
}
