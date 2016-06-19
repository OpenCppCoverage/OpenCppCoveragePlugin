using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace OpenCppCoverage.VSPackage.CoverageTree
{
    [Guid("2731AD3B-8E0A-46D2-B8F6-CA40C387732F")]
    class CoverageTreeToolWindow : ToolWindowPane
    {
        public CoverageTreeToolWindow() : base(null)
        {
            this.Caption = "CoverageTree";

            // This is the user control hosted by the tool window; 
            // Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. 
            // This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var coverageTreeControl = new CoverageTreeControl();
            this.Content = coverageTreeControl;
        }
    }
}
