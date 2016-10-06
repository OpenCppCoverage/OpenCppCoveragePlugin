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
using OpenCppCoverage.VSPackage.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VSPackage_IntegrationTests
{
    [TestClass()]
    public class PathHelperTests
    {
        //---------------------------------------------------------------------
        [TestMethod]
        public void TestComputeCommonFolders()
        {
            const string file2 = @"C:\Dev\Folder2\Folder3\File2.txt";
            const string file4 = @"C:\Dev\Folder1\File4.txt";
            
            var paths = new List<string>
            {
                @"C:\Dev\Folder1\Folder2\Folder3\File1.txt",
                file2,
                @"C:\Dev\Folder1\Folder2\File3.txt",
                file4
            };
              
            var commonFolders = PathHelper.ComputeCommonFolders(paths);

            var expectedFolders = new List<string> {
                Path.GetDirectoryName(file4),
                Path.GetDirectoryName(file2)};
            CollectionAssert.AreEqual(expectedFolders, commonFolders.ToList());
        }
    }
}
