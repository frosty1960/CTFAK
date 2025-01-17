﻿using System;
using System.Collections.Generic;
using System.IO;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders.Events;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class Events:DataLoader
    {
        public const string EventData = "Evts";
        public const string CommentData = "Rems";
        public const string ObjectData = "EvOb";
        public const string EventEditorData = "EvCs";
        public const string ObjectListData = "EvEd";
        public const string TimeListData = "EvEd";
        public const string EditorPositionData = "EvTs";
        public const string EditorLineData = "EvLs";
        public const string UnknownEventData = "E2Ts";
        public const string EventEnd ="!DNE";
        public List<EventGroup> Items;
        public ushort Version;
        public ushort FrameType;
        public List<Comment> Comments;
        public List<EventObject> Objects;
        public ushort ConditionWidth;
        public ushort ObjectHeight;
        public List<ushort> ObjectTypes;
        public List<ushort> ObjectHandles;
        public List<ushort> ObjectFlags;
        public List<string> Folders;
        public uint X;
        public uint Y;
        public uint CaretType;
        public uint CaretX;
        public uint CaretY;
        public uint LineY;
        public uint LineItemType;
        public uint EventLine;
        public uint EventLineY;
        public byte[] Saved;
        public int EditorDataUnk;
        public uint EventDataLen;
        public uint CommentDataLen;
        public byte[] _cache;
        public bool _ifMFA;

        public Events(ByteReader reader) : base(reader)
        {
        }

        public Events(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            Version = Reader.ReadUInt16();
            FrameType = Reader.ReadUInt16();
            Items = new List<EventGroup>();
            
            while (true)
            {
                
                string name = Reader.ReadAscii(4);
                if (name == EventData)
                {
                    EventDataLen = Reader.ReadUInt32();
                    uint end = (uint) (Reader.Tell() + EventDataLen);
                    while (true)
                    {
                        EventGroup evGrp = new EventGroup(Reader);
                        evGrp.isMFA = true;
                        evGrp.Read();
                        Items.Add(evGrp);
                        if (Reader.Tell() >= end) break;
                    }
                }
                else if (name == CommentData)
                {
                    try
                    {
                        CommentDataLen = Reader.ReadUInt32();
                        Comments = new List<Comment>();
                        Comment comment = new Comment(Reader);
                        comment.Read();
                        Comments.Add(comment);
                    }
                    catch
                    {
                        //What the fuck?
                        
                        /*
                        import code
                        code.interact(local = locals())
                        */
                    }
                }
                else if (name == ObjectData)
                {
                    Objects = new List<EventObject>();
                    uint len = Reader.ReadUInt32();
                    for (int i = 0; i < len; i++)
                    {
                        EventObject eventObject = new EventObject(Reader);
                        eventObject.Read();
                        Objects.Add(eventObject);
                        
                    }
                }
                else if (name == EventEditorData)
                {
                    EditorDataUnk = Reader.ReadInt32();
                    ConditionWidth = Reader.ReadUInt16();
                    ObjectHeight = Reader.ReadUInt16();
                    Reader.Skip(12);
                }
                else if (name == ObjectListData)
                {
                    short count = Reader.ReadInt16();
                    short realCount = count;
                    if (count == -1)
                    {
                        realCount = Reader.ReadInt16();
                    }

                    ObjectTypes = new List<ushort>();
                    for (int i = 0; i < realCount; i++)
                    {
                        ObjectTypes.Add(Reader.ReadUInt16());
                    }
                    ObjectHandles = new List<ushort>();
                    for (int i = 0; i < realCount; i++)
                    {
                        ObjectHandles.Add(Reader.ReadUInt16());
                    }
                    ObjectFlags = new List<ushort>();
                    for (int i = 0; i < realCount; i++)
                    {
                        ObjectFlags.Add(Reader.ReadUInt16());
                    }

                    if (count == -1)
                    {
                        Folders = new List<string>();
                        var folderCount = Reader.ReadUInt16();
                        for (int i = 0; i < folderCount; i++)
                        {
                            Folders.Add( Reader.AutoReadUnicode());
                        }
                    }
                }
                else if (name == TimeListData)
                {
                    throw new NotImplementedException("I don't like no timelist");
                }
                else if (name == EditorPositionData)
                {
                    if(Reader.ReadUInt16()!=1)//throw new NotImplementedException("Invalid chunkversion");
                    X = Reader.ReadUInt32();
                    Y = Reader.ReadUInt32();
                    CaretType = Reader.ReadUInt32();
                    CaretX = Reader.ReadUInt32();
                    CaretY = Reader.ReadUInt32();
                }
                else if (name == EditorLineData)
                {
                    if(Reader.ReadUInt16()!=1)//throw new NotImplementedException("Invalid chunkversion");
                    LineY = Reader.ReadUInt32();
                    LineItemType = Reader.ReadUInt32();
                    EventLine = Reader.ReadUInt32();
                    EventLineY = Reader.ReadUInt32();
                }
                else if (name == UnknownEventData)
                {
                    Reader.Skip(12);
                }
                else if (name == EventEnd)
                {
                    // _cache = Reader.ReadBytes(122);
                    
                    break;
                }
                else Logger.Log("UnknownGroup: "+name);//throw new NotImplementedException("Fuck Something is Broken: "+name);

            }
        }

        public override void Write(ByteWriter Writer)
        {
            
            Writer.WriteUInt16(Version);
            Writer.WriteUInt16(FrameType);
            if (Items.Count>0)
            {
                //Console.WriteLine("Writing EventData");
                Writer.WriteAscii(EventData);

                ByteWriter newWriter = new ByteWriter(new MemoryStream());
                //Writer.WriteUInt32(EventDataLen);
                
                foreach (EventGroup eventGroup in Items)
                {
                    eventGroup.isMFA = true;
                    eventGroup.Write(newWriter);
                }
                

                Writer.WriteUInt32((uint) newWriter.BaseStream.Position);
                Writer.WriteWriter(newWriter);
                
            }

            
            if (Objects?.Count>0)
            {
                //Console.WriteLine("Writing EventObjects");
                Writer.WriteAscii(ObjectData);
                Writer.WriteUInt32((uint) Objects.Count);
                foreach (EventObject eventObject in Objects)
                {
                    eventObject.Write(Writer);
                }
            }
            if (ObjectTypes != null)
            {
                Writer.WriteAscii(ObjectListData);
                Writer.WriteInt16(-1);
                Writer.WriteInt16((short) ObjectTypes.Count);
                foreach (ushort objectType in ObjectTypes)
                {
                    Writer.WriteUInt16(objectType);
                }

                foreach (ushort objectHandle in ObjectHandles)
                {
                    Writer.WriteUInt16(objectHandle);
                }

                foreach (ushort objectFlag in ObjectFlags)
                {
                    Writer.WriteUInt16(objectFlag);
                }

                Writer.WriteUInt16((ushort) Folders.Count);
                foreach (string folder in Folders)
                {
                    Writer.AutoWriteUnicode(folder);
                }
            }
            
            
            
            

            // if (X != 0)
            {
                Writer.WriteAscii(EditorPositionData);
                Writer.WriteInt16(10);
                Writer.WriteInt32((int) X);
                Writer.WriteInt32((int) Y);
                Writer.WriteUInt32(CaretType);
                Writer.WriteUInt32(CaretX);
                Writer.WriteUInt32(CaretY);
            }
            // if (LineY != 0)
            {
                Writer.WriteAscii(EditorLineData);
                Writer.WriteInt16(10);
                Writer.WriteUInt32(LineY);
                Writer.WriteUInt32(LineItemType);
                Writer.WriteUInt32(EventLine);
                Writer.WriteUInt32(EventLineY);
            }
            Writer.WriteAscii(UnknownEventData);
            Writer.WriteInt8(8);
            Writer.Skip(9);
            Writer.WriteInt16(0);
            
            Writer.WriteAscii(EventEditorData);
            // Writer.Skip(4+2*2+4*3);
            Writer.WriteInt32(EditorDataUnk);
            Writer.WriteInt16((short) ConditionWidth);
            Writer.WriteInt16((short) ObjectHeight);
            Writer.Skip(12);


            Writer.WriteAscii(EventEnd);
            
            // Writer.WriteBytes(_cache);



            //TODO: Fix commented part
            // 

            //
            // if (Comments != null)
            // {
            //     Console.WriteLine("Writing Comments");
            //     Writer.WriteAscii(CommentData);
            //     foreach (Comment comment in Comments)
            //     {
            //         comment.Write(Writer);
            //     }
            // }

        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Comment:DataLoader
    {
        public uint Handle;
        public string Value;

        public Comment(ByteReader reader) : base(reader)
        {
        }

        public Comment(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Handle = Reader.ReadUInt32();
            Value = Helper.AutoReadUnicode(Reader);
        }

        public override void Write(ByteWriter Writer)
        {
           Writer.WriteUInt32(Handle);
           Writer.AutoWriteUnicode(Value);
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }

    public class EventObject : DataLoader
    {
        public uint Handle;
        public ushort ObjectType;
        public ushort ItemType;
        public string Name;
        public string TypeName;
        public ushort Flags;
        public uint ItemHandle;
        public uint InstanceHandle;
        public string Code;
        public string IconBuffer;
        public ushort SystemQualifier;

        public EventObject(ByteReader reader) : base(reader)
        {
        }

        public EventObject(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Handle = Reader.ReadUInt32();
            ObjectType = Reader.ReadUInt16();
            ItemType = Reader.ReadUInt16();
            Name = Reader.AutoReadUnicode();//Not Sure
            TypeName = Reader.AutoReadUnicode();//Not Sure
            Flags = Reader.ReadUInt16();
            if (ObjectType == 1)//FrameItemType
            {
                ItemHandle = Reader.ReadUInt32();
                InstanceHandle = Reader.ReadUInt32();
            }
            else if (ObjectType == 2)//ShortcutItemType
            {
                Code = Reader.ReadAscii(4);
                Logger.Log("Code: "+Code);
                if (Code == "OIC2")//IconBufferCode
                {
                    IconBuffer = Reader.AutoReadUnicode();
                }
            }
            if (ObjectType == 3) //SystemItemType
            {
                SystemQualifier = Reader.ReadUInt16();
            }
            
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt32(Handle);
            Writer.WriteUInt16(ObjectType);
            Writer.WriteUInt16(ItemType);
            Writer.AutoWriteUnicode(Name);//Not Sure
            Writer.AutoWriteUnicode(TypeName);//Not Sure
            Writer.WriteUInt16(Flags);
            if (ObjectType == 1)
            {
                Writer.WriteUInt32(ItemHandle);
                Writer.WriteUInt32(InstanceHandle);
            }
            else if (ObjectType == 2)
            {
                // Code = "OIC2";
                Writer.WriteAscii(Code);
                if (Code == "OIC2")
                {
                    Writer.AutoWriteUnicode(IconBuffer);
                }
            }
            if (ObjectType == 3)
            {
                Writer.WriteUInt16(SystemQualifier);
            }
            
            
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}