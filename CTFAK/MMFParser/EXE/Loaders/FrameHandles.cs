﻿using System.Collections.Generic;
using System.Linq;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class FrameHandles:ChunkLoader
    {
        public Dictionary<int,int> Items;

        public FrameHandles(ByteReader reader) : base(reader)
        {
        }

        public FrameHandles(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            var len = Reader.Size() / 2;
            Items = new Dictionary<int,int>();
            for (int i = 0; i < len; i++)
            {
                var handle = Reader.ReadInt16();
                Items.Add(i,handle);
            }

        }

        public override void Print(bool ext){}
        public override string[] GetReadableData() => null;

    }
}