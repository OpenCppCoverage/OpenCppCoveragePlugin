using System.Collections.Generic;

namespace OpenCppCoverage.VSPackage.CoverageData
{
    class CoverageResult
    {
        public CoverageResult(
            ProtoBuff.CoverageData coverageData, 
            IEnumerable<ProtoBuff.ModuleCoverage> modules)
        {
            this.ExitCode = coverageData.ExitCode;
            this.Modules = modules;
        }

        public int ExitCode { get; private set; }
        public IEnumerable<ProtoBuff.ModuleCoverage> Modules { get; private set; }
    }
}
