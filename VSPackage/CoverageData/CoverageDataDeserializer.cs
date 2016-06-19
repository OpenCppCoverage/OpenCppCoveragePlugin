using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenCppCoverage.VSPackage.CoverageData
{
    class CoverageDataDeserializer
    {
        public static uint FileTypeId = 1351727964;

        //---------------------------------------------------------------------
        public CoverageResult Deserialize(Stream stream)
        {
            var codedInputStream = CodedInputStream.CreateInstance(stream);
            
            uint fileTypeId = 0;
            if (!codedInputStream.ReadUInt32(ref fileTypeId) || fileTypeId != FileTypeId)
                throw new Exception("Binary format is not valid.");

            var coverageData = ReadMessage(codedInputStream, ProtoBuff.CoverageData.ParseFrom);

            var modules = new List<ProtoBuff.ModuleCoverage>();
            for (ulong i = 0; i < coverageData.ModuleCount; ++i)
                modules.Add(ReadMessage(codedInputStream, ProtoBuff.ModuleCoverage.ParseFrom));

            return new CoverageResult(coverageData, modules);
        }

        //---------------------------------------------------------------------
        static T ReadMessage<T>(CodedInputStream input, Func<byte[], T> parseFrom)
        {
            uint size = 0;

            if (!input.ReadUInt32(ref size))
                throw new Exception("Cannot read message size.");

            var data = input.ReadRawBytes((int)size);
            return parseFrom(data);
        }
    }
}
