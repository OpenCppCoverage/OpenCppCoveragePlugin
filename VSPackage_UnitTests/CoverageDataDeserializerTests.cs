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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage.CoverageData;
using OpenCppCoverage.VSPackage.CoverageData.ProtoBuff;
using System.IO;
using System.Linq;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class CoverageDataDeserializerTests
    {
        //---------------------------------------------------------------------
        static readonly string coverageName = "coverageName";
        static readonly ulong moduleCount = 1;
        static readonly int exitCode = 42;
        static readonly string modulePath = "modulePath";
        static readonly string filePath = "filePath";
        static readonly bool hasBeenExecuted = true;
        static readonly uint lineNumber = 10;

        //---------------------------------------------------------------------
        [TestMethod]
        public void Deserialize()
        {
            using (var stream = new MemoryStream())
            {
                WriteCoverageData(stream);

                var deserializer = new CoverageDataDeserializer();
                var coverageResult = deserializer.Deserialize(stream);

                Assert.AreEqual(coverageName, coverageResult.CoverageData.Name);
                Assert.AreEqual(exitCode, coverageResult.CoverageData.ExitCode);

                var module = coverageResult.Modules.First();
                Assert.AreEqual(modulePath, module.Path);

                var file = module.FilesList.First();
                Assert.AreEqual(filePath, file.Path);

                var line = file.LinesList.First();
                Assert.AreEqual(hasBeenExecuted, line.HasBeenExecuted);
                Assert.AreEqual(lineNumber, line.LineNumber);
            }
        }

        //---------------------------------------------------------------------
        static void WriteCoverageData(Stream stream)
        {
            var outputStream = CodedOutputStream.CreateInstance(stream);

            outputStream.WriteRawVarint32(CoverageDataDeserializer.FileTypeId);
            WriteCoverageDataOnly(outputStream);
            WriteModule(outputStream);

            outputStream.Flush();
            stream.Position = 0;
        }

        //---------------------------------------------------------------------
        static void WriteCoverageDataOnly(CodedOutputStream outputStream)
        {
            var coverageData = CoverageData.CreateBuilder();

            coverageData.SetName(coverageName);
            coverageData.SetModuleCount(moduleCount);
            coverageData.SetExitCode(exitCode);
            WriteMessage(outputStream, coverageData.Build());
        }

        //---------------------------------------------------------------------
        static void WriteModule(CodedOutputStream outputStream)
        {
            var line = LineCoverage.CreateBuilder();
            line.SetHasBeenExecuted(hasBeenExecuted);
            line.SetLineNumber(lineNumber);

            var file = FileCoverage.CreateBuilder();
            file.SetPath(filePath);
            file.AddLines(line);

            var module = ModuleCoverage.CreateBuilder();
            module.SetPath(modulePath);
            module.AddFiles(file);

            WriteMessage(outputStream, module.Build());
        }

        //---------------------------------------------------------------------
        static void WriteMessage(CodedOutputStream outputStream, IMessage message)
        {
            outputStream.WriteRawVarint32((uint)message.SerializedSize);
            message.WriteTo(outputStream);
        }
    }
}