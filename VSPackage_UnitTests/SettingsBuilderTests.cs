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

using EnvDTE80;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.VCProjectEngine;
using Moq;
using OpenCppCoverage.VSPackage;
using OpenCppCoverage.VSPackage.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VSPackage_UnitTests;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class SettingsBuilderTests
    {
        //---------------------------------------------------------------------
        static VCFile BuildVCFile(string path)
        {
            var file = new Mock<VCFile>();

            file.Setup(f => f.FullPath).Returns(path);
            return file.Object;
        }

        public class ConfigurationMock
        {
            public class DebugSettingsMock
            {
                public string Command { get; set; }
                public string CommandArguments { get; set; }
                public string WorkingDirectory { get; set; }
            }

            public string PrimaryOutput { get; set; }
            public DebugSettingsMock DebugSettings { get; set; } = new DebugSettingsMock();
            public string Evaluate(string str) { return str; }
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void TestComputeCommonFolders()
        {
            var builder = new DteMockBuilder();

            const string file2 = @"C:\Dev\Folder2\Folder3\File2.txt";
            const string file4 = @"C:\Dev\Folder1\File4.txt";
            
            builder.VcFiles = new List<VCFile>
            {
                BuildVCFile(@"C:\Dev\Folder1\Folder2\Folder3\File1.txt"),
                BuildVCFile(file2),
                BuildVCFile(@"C:\Dev\Folder1\Folder2\File3.txt"),
                BuildVCFile(file4)               
            };
              
            var solution = builder.BuildSolutionMock();
            var configurationManager = new Mock<IConfigurationManager>();
            string output = "output";
            var configuration = new ConfigurationMock { PrimaryOutput = output };
            configurationManager.Setup(c => c.FindConfiguration(
                It.IsAny<SolutionConfiguration2>(), It.IsAny<ExtendedProject>()))
                .Returns(new DynamicVCConfiguration(configuration));
            configurationManager.Setup(c => c.GetConfiguration(
                It.IsAny<SolutionConfiguration2>(), It.IsAny<ExtendedProject>()))
                .Returns(new DynamicVCConfiguration(configuration));

            // $$ TODO: Fix test.
            //var settingsBuilder = new StartUpProjectSettingsBuilder(
            //    solution.Object, 
            //    configurationManager.Object);
            //var settings = settingsBuilder.ComputeOptionalSettings();

            //var expectedFolders = new List<string> { 
            //    Path.GetDirectoryName(file4) + Path.DirectorySeparatorChar, 
            //    Path.GetDirectoryName(file2) + Path.DirectorySeparatorChar };
            //var project = settings.CppProjects.Single();
            //CollectionAssert.AreEqual(expectedFolders, project.SourcePaths.ToList());
            //Assert.AreEqual(output, project.ModulePath);
        }
    }
}
