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

                Assert.AreEqual(exitCode, coverageResult.ExitCode);

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

            coverageData.SetName("NotUsed");
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