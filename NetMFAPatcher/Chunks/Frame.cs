﻿using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Chunks
{
    class FrameName : StringChunk
    {
    }
    class FramePassword : StringChunk
    {
    }
    public class Frame : ChunkLoader
    {
        ByteIO reader;
        public string name;
        public string password;
        public int width;
        public int height;
        //background, idk what type is it
        //flags
        int top;
        int bottom;
        int left;
        int right;

        

        public override void Print()
        {
            
        }

        public override void Read()
        {
            var FrameReader = new ByteIO(chunk.chunk_data);
            var chunks = new ChunkList();
            chunks.verbose = false;
            chunks.Read(FrameReader);
            var name = chunks.get_chunk<FrameName>();
            Logger.Log(name.value);

            
        }
    }


}