// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace OpenCppCoverage.VSPackage
{
    static class PkgCmdIDList
    {
        public const uint RunOpenCppCoverageCommand =        0x100;

        public const uint RunCoverageInProjectFromProjectCtx = 0x300;
    };
}