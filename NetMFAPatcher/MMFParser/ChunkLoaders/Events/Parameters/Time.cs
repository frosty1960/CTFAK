﻿using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Time : ParameterCommon
    {
        public int Timer;
        public int Loops;

        public Time(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            Timer = Reader.ReadInt32();
            Loops = Reader.ReadInt32();
            
        }
        public override string ToString()
        {

            return $"Time time: {Timer} loops: {Loops}";
        }
    }
}