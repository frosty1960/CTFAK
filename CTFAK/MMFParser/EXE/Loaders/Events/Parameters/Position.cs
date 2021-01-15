﻿using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Position : ParameterCommon
    {
        public int ObjectInfoParent;
        public ushort Flags;
        public int X;
        public int Y;
        public int Slope;
        public int Angle;
        public float Direction;
        public int TypeParent;
        public int ObjectInfoList;
        public int Layer;            

        public Position(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            ObjectInfoParent = Reader.ReadInt16();
            Flags = Reader.ReadUInt16();
            X = Reader.ReadInt16();
            Y = Reader.ReadInt16();
            Slope = Reader.ReadInt16();
            Angle = Reader.ReadInt16();
            Direction = Reader.ReadSingle();
            TypeParent = Reader.ReadInt16();
            ObjectInfoList = Reader.ReadInt16();
            Layer = Reader.ReadInt16();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) ObjectInfoParent);
            Writer.WriteUInt16(Flags);
            Writer.WriteInt16((short) X);
            Writer.WriteInt16((short) Y);
            Writer.WriteInt16((short) Slope);
            Writer.WriteInt16((short) Angle);
            Writer.WriteSingle(Direction);
            Writer.WriteInt16((short) TypeParent);
            Writer.WriteInt16((short) ObjectInfoList);
            Writer.WriteInt16((short) Layer);
        }

        public override string ToString()
        {
            return $"Object X:{X} Y:{Y} Angle:{Angle} Direction:{Direction} Parent:{ObjectInfoList}";
        }
    }
}
