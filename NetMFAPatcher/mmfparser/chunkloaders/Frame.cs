﻿
using NetMFAPatcher.MMFParser.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters;
using NetMFAPatcher.MMFParser.MFALoaders;
using NetMFAPatcher.Utils;
using ChunkList = NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    class FrameName : StringChunk
    {
        public FrameName(ByteIO reader) : base(reader)
        {
        }

        public FrameName(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    class FramePassword : StringChunk
    {
        public FramePassword(ByteIO reader) : base(reader)
        {
        }

        public FramePassword(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    public class Frame : ChunkLoader
    {
        public string Name;
        public string Password;
        public int Width;
        public int Height;
        public byte[] Background;
        public int Flags;
        public int CountOfObjs;
        int _top;
        int _bottom;
        int _left;
        int _right;
        public ChunkList Chunks;
        public FrameHeader Header;
        public ObjectInstances Objects;


        public override void Print(bool ext)
        {
            Logger.Log($"Frame: {Name}", true, ConsoleColor.Green);
            Logger.Log($"   Password: {(Password!=null ? Password : "None")}", true, ConsoleColor.Green);
            Logger.Log($"   Size: {Width}x{Height}", true, ConsoleColor.Green);
            Logger.Log($"   Objects: {CountOfObjs}", true, ConsoleColor.Green);
            Logger.Log($"-------------------------", true, ConsoleColor.Green);
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Size: {Width}x{Height}",
                $"Objects: {CountOfObjs}"
            
            };
        }

        public override void Read()
        {
            var frameReader = new ByteIO(Chunk.ChunkData);
            Chunks = new ChunkList();

            Chunks.Verbose = false;
            Chunks.Read(frameReader);

            var name = Chunks.get_chunk<FrameName>();
            if (name != null) //Just to be sure
            {
                this.Name = name.Value;
            }
            var password = Chunks.get_chunk<FramePassword>();
            if (password != null) //Just to be sure
            {
                this.Password = password.Value;
            }
            Header = Chunks.get_chunk<FrameHeader>();
            Width = Header.Width;
            Height = Header.Height;
            Background = Header.Background;
            //Flags = header.Flags;
            Objects = Chunks.get_chunk<ObjectInstances>();
            if(Objects!=null)
            {
                CountOfObjs = Objects.CountOfObjects;              
            }
            






            foreach (var item in Chunks.Chunks)
            {
                //Directory.CreateDirectory($"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}");
                //string path = $"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}\\{chunk.name}.chunk";
                //File.WriteAllBytes(path, item.chunk_data);

            }
            
            


        }

        public Frame(ByteIO reader) : base(reader)
        {
        }

        public Frame(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    public class FrameHeader : ChunkLoader
    {
        public int Width;
        public int Height;
        public BitDict Flags = new BitDict(new string[]
        {
            "XCoefficient",
            "YCoefficient",
            "DoNotSaveBackground",
            "Wrap",
            "Visible",
            "WrapHorizontally",
            "WrapVertically","","","","","","","","","",
            "Redraw",
            "ToHide",
            "ToShow"
                
        });
        public byte[] Background;
        public FrameHeader(ByteIO reader) : base(reader)
        {
        }

        public FrameHeader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
        return new string[]
            {
              $"Size: {Width}x{Height}",
              $"Flags:;{Flags.ToString()}"
 
            };
        }

        public override void Read()
        {
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Background = Reader.ReadBytes(4);
            Flags.flag = Reader.ReadUInt32();
            
            

        }
    }

    public class ObjectInstances : ChunkLoader
    {
        
        public int CountOfObjects=0;
        public List<ObjectInstance> Items = new List<ObjectInstance>();

        public ObjectInstances(ByteIO reader) : base(reader)
        {
        }

        public ObjectInstances(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {

        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of objects: {CountOfObjects}"
            };
        }

        public override void Read()
        {
            
            CountOfObjects = (int)Reader.ReadUInt32();
            for (int i = 0; i < CountOfObjects; i++)
            {
                var item = new ObjectInstance(Reader);
                item.Read();
                Items.Add(item);
            }
            Reader.Skip(4);
        }
    }

    public class ObjectInstance : ChunkLoader
    {
        public ushort Handle;
        public ushort ObjectInfo;
        public int X;
        public int Y;
        public short ParentType;
        public short Layer;
        public string Name;
        public short ParentHandle;

        public ObjectInstance(ByteIO reader) : base(reader)
        {
        }

        public ObjectInstance(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
           
            Handle = Reader.ReadUInt16();
            //if (Handle > 0) Handle -= 1;
            ObjectInfo = Reader.ReadUInt16();
            X = Reader.ReadInt32();
            Y = Reader.ReadInt32();
            ParentType = Reader.ReadInt16();
            ParentHandle = Reader.ReadInt16();
            Layer = Reader.ReadInt16();
            Reader.Skip(2);
            //-------------------------
            if (FrameItem != null) Name = FrameItem.Name;
            else Name = $"UNKNOWN-{Handle}";
           Console.WriteLine("ObjectInfoHandle: "+Handle);

        }

        public ObjectInfo FrameItem
        {
            get
            {
                return Exe.LatestInst.GameData.GameChunks.get_chunk<FrameItems>().GetItemByHandle(Handle);
            }
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Type:{(Constants.ObjectType)FrameItem.ObjectType} - {FrameItem.ObjectType}",
                $"Position: {X,5}x{Y,5}",
                $"Size: CUMxCUM"

            };
        }
    }

    class Layer : ChunkLoader
    {
        public Layer(ByteIO reader) : base(reader)
        {
        }

        public Layer(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }


}