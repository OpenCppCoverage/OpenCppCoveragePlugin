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
using Moq;
using OpenCppCoverage.VSPackage.Helper;
using OpenCppCoverage.VSPackage.Settings.UI;
using System;

namespace VSPackage_UnitTests
{
    [TestClass()]
    public class BasicSettingControllerTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void ProgramToRunCommand()
        {
            var fileSystemDialog = new Mock<IFileSystemDialog>();
            var controller = new BasicSettingController(fileSystemDialog.Object);
            var file = "file";

            fileSystemDialog.Setup(
                f => f.SelectFile(It.IsAny<string>(), It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>(
                    (filter, onSeletedFilename) => onSeletedFilename(file));
            controller.ProgramToRunCommand.Execute(null);
            Assert.AreEqual(file, controller.ProgramToRun);
        }

        //---------------------------------------------------------------------
        [TestMethod]
        public void WorkingDirectoryCommand()
        {
            var fileSystemDialog = new Mock<IFileSystemDialog>();
            var controller = new BasicSettingController(fileSystemDialog.Object);
            var path = "path";

            fileSystemDialog.Setup(
                f => f.SelectFolder(It.IsAny<Action<string>>()))
                    .Callback<Action<string>>(
                        (onSeletedPath) => onSeletedPath(path));
            controller.WorkingDirectoryCommand.Execute(null);
            Assert.AreEqual(path, controller.WorkingDirectory);
        }        
    }
}
