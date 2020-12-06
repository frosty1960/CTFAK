﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class StringChunk : ChunkLoader
    {
        public string Value;


        public override void Read()
        {
            Reader = new ByteIO(Chunk.ChunkData);
            Value = Reader.ReadWideString();
        }

        public override void Print(bool ext)
        {
            Logger.Log($"{Chunk.Name} contains:  {Value}\n",true,ConsoleColor.DarkCyan);
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Value: {Value}"
            };

        }


        public StringChunk(ByteIO reader) : base(reader)
        {
        }

        public StringChunk(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppName : StringChunk
    {
        public AppName(ByteIO reader) : base(reader)
        {
        }

        public AppName(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppAuthor : StringChunk
    {
        public AppAuthor(ByteIO reader) : base(reader)
        {
        }

        public AppAuthor(Chunk chunk) : base(chunk)
        {
        }
    }

    class ExtPath : StringChunk
    {
        public ExtPath(ByteIO reader) : base(reader)
        {
        }

        public ExtPath(Chunk chunk) : base(chunk)
        {
        }
    }

    class EditorFilename : StringChunk
    {
        public EditorFilename(ByteIO reader) : base(reader)
        {
        }

        public EditorFilename(Chunk chunk) : base(chunk)
        {
        }
    }

    class TargetFilename : StringChunk
    {
        public TargetFilename(ByteIO reader) : base(reader)
        {
        }

        public TargetFilename(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppDoc : StringChunk
    {
        public AppDoc(ByteIO reader) : base(reader)
        {
        }

        public AppDoc(Chunk chunk) : base(chunk)
        {
        }
    }

    class AboutText : StringChunk
    {
        public AboutText(ByteIO reader) : base(reader)
        {
        }

        public AboutText(Chunk chunk) : base(chunk)
        {
        }
    }

    class Copyright : StringChunk
    {
        public Copyright(ByteIO reader) : base(reader)
        {
        }

        public Copyright(Chunk chunk) : base(chunk)
        {
        }
    }

    class DemoFilePath : StringChunk
    {
        public DemoFilePath(ByteIO reader) : base(reader)
        {
        }

        public DemoFilePath(Chunk chunk) : base(chunk)
        {
        }
    }
}