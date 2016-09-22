// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2016 OpenCppCoverage
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenCppCoverage.VSPackage;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class CommandLineBuilderTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void AppendArgument()
        {
            Assert.AreEqual("argument",
                new CommandLineBuilder().AppendArgument("argument", null).GetCommandLine());
            Assert.AreEqual(@"argument ""value""",
                new CommandLineBuilder().AppendArgument("argument", "value").GetCommandLine());
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void AppendArgumentCollection()
        {
            var builder = new CommandLineBuilder();
            builder.AppendArgumentCollection("argument", new List<string> { "value1", "value2" });

            Assert.AreEqual(@"argument ""value1"" argument ""value2""", builder.GetCommandLine());
        }
        
        //---------------------------------------------------------------------
        [TestMethod]
        public void EscapeValue()
        {
            foreach (var value in new List<string> {
                "value", @"va\lue", @"va""lue", @"va\""lue", @"va\\\""lue", @"value\", @"value\\" })
            {
                Assert.AreEqual(value, EscapeAndGetProcessOutput(value));
            }
        }

        //---------------------------------------------------------------------
        string EscapeAndGetProcessOutput(string str)
        {
            var escapedValue = CommandLineBuilder.EscapeValue(str);
            var output = GetOutputForCommandLineArg(escapedValue);

            return output.Single();
        }

        //---------------------------------------------------------------------
        static List<string> GetOutputForCommandLineArg(string arguments)
        {
            var lines = new List<string>();
            var assemblyPath = Assembly.GetCallingAssembly().Location;
            var startInfo = new ProcessStartInfo(assemblyPath, arguments);
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            var process = Process.Start(startInfo);
            var standardOutput = process.StandardOutput;
            while (!standardOutput.EndOfStream)
                lines.Add(standardOutput.ReadLine());

            return lines;
        }
    }
}