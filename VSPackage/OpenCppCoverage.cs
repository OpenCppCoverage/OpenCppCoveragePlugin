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

using OpenCppCoverage.VSPackage.Settings;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OpenCppCoverage.VSPackage
{
    class OpenCppCoverage
    {
        readonly OutputWindowWriter outputWindowWriter;

        //---------------------------------------------------------------------
        public OpenCppCoverage(OutputWindowWriter outputWindowWriter)
        {
            this.outputWindowWriter = outputWindowWriter;
        }

        //---------------------------------------------------------------------
        public Task RunCodeCoverageAsync(MainSettings settings)
        {
            var basicSettings = settings.BasicSettings;
            var fileName = GetOpenCppCoveragePath(basicSettings.ProgramToRun);
            var arguments = OpenCppCoverageCmdLine.Build(settings);

            this.outputWindowWriter.WriteLine("Run:");
            this.outputWindowWriter.WriteLine(string.Format(@"""{0}"" {1}",
                fileName, arguments));

            // Run in a new thread to not block UI thread.
            return Task.Run(() =>
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = false;

                    if (!String.IsNullOrEmpty(basicSettings.WorkingDirectory))
                        process.StartInfo.WorkingDirectory = basicSettings.WorkingDirectory;
                    process.Start();
                    process.WaitForExit();
                }
            });
        }

        //---------------------------------------------------------------------
        string GetOpenCppCoveragePath(string commandPath)
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyFolder = Path.GetDirectoryName(assemblyLocation);
            var openCppCovergeFolder = 
                Is64bitsExecutable(commandPath) ? 
                    "OpenCppCoverage-x64" : "OpenCppCoverage-x86";

            return Path.Combine(assemblyFolder, openCppCovergeFolder, "OpenCppCoverage.exe");
        }

        //---------------------------------------------------------------------
        bool Is64bitsExecutable(string commandPath)
        {
            NativeMethods.BinaryType binaryType;

            if (NativeMethods.GetBinaryType(commandPath, out binaryType) == 0)
                throw new VSPackageException("Invalid executable: " + commandPath);

            outputWindowWriter.WriteLine(
                string.Format("{0} has binary type: {1}", commandPath, binaryType.ToString()));

            if (binaryType != NativeMethods.BinaryType.SCS_64BIT_BINARY 
                && binaryType != NativeMethods.BinaryType.SCS_32BIT_BINARY)
            {
                throw new VSPackageException(
                    string.Format("Invalid binary format {0} for {1}", binaryType.ToString(), commandPath));
            }

            return binaryType == NativeMethods.BinaryType.SCS_64BIT_BINARY;
        }

        //---------------------------------------------------------------------
        class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
            public extern static Int32 GetBinaryType(string lpApplicationName, out BinaryType lpBinaryType);

            public enum BinaryType : uint
            {
                SCS_32BIT_BINARY = 0, // A 32-bit Windows-based application
                SCS_64BIT_BINARY = 6, // A 64-bit Windows-based application.
                SCS_DOS_BINARY = 1, // An MS-DOS – based application
                SCS_OS216_BINARY = 5, // A 16-bit OS/2-based application
                SCS_PIF_BINARY = 3, // A PIF file that executes an MS-DOS – based application
                SCS_POSIX_BINARY = 4, // A POSIX – based application
                SCS_WOW_BINARY = 2 // A 16-bit Windows-based application
            }
        }
    }
}
