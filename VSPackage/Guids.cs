// Guids.cs
// MUST match guids.h
using System;

namespace OpenCppCoverage.VSPackage
{
    static class GuidList
    {
        public const string guidVSPackagePkgString = "c6a77aca-f53c-4cd1-97d7-0ed595751347";
        public const string guidVSPackageCmdSetString = "fe1f442f-480d-4a2b-bf8a-adc8a0fc569d";
        public const string guidStartCovInProjCTXCmdSetString = "7FFD1AD4-5BFD-4F5C-9EBC-52AFE39D4BFE";
        public static readonly Guid guidVSPackageCmdSet = new Guid(guidVSPackageCmdSetString);
        public static readonly Guid guidStartCovInProjCTXCmdSet = new Guid(guidStartCovInProjCTXCmdSetString);
    };
}