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

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading.Tasks;

namespace OpenCppCoverage.VSPackage
{
    class ErrorHandler
    {
        //---------------------------------------------------------------------
        public ErrorHandler(IVsUIShell uiShell)
        {
            uiShell_ = uiShell;
        }

        //---------------------------------------------------------------------
        public OutputWindowWriter OutputWriter { get; set; }

        //---------------------------------------------------------------------
        public async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (VSPackageException e)
            {
                if (OutputWriter != null)
                    OutputWindowWriter.WriteLine("ERROR: " + e.Message);
                ShowMessage(e.Message);
            }
            catch (Exception e)
            {
                if (OutputWriter != null && OutputWindowWriter.WriteLine("ERROR: " + e.ToString()))             
                    ShowMessage("Unknow error. Please see the output console for more information.");
                else
                    ShowMessage(e.ToString());
                OutputWindowWriter.WriteLine("ERROR: " + e.Message);
            }
        }

        //---------------------------------------------------------------------
        public void Execute(Action action)
        {
            ExecuteAsync(() =>
            {
                action();
                return Task.FromResult(0);
            }).Wait();
        }

        //---------------------------------------------------------------------
        void ShowMessage(string message)
        {
            Guid clsid = Guid.Empty;
            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell_.ShowMessageBox(
                       0,
                       ref clsid,
                       "OpenCppCoverage",
                       message,
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0, // false
                       out result));
        }

        readonly IVsUIShell uiShell_;
    }
}
