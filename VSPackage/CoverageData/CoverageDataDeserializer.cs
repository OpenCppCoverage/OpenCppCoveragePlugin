// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2014 OpenCppCoverage
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
