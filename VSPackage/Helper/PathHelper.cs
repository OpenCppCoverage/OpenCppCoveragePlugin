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

using System.Collections.Generic;
using System.IO;

namespace OpenCppCoverage.VSPackage.Helper
{
    static class PathHelper
    {
        //---------------------------------------------------------------------       
        public static IEnumerable<string> ComputeCommonFolders(IEnumerable<string> filePaths)
        {
            var commonFolders = new List<string>();

            foreach (var path in filePaths)
                commonFolders.Add(Path.GetDirectoryName(path));
            commonFolders.Sort();
            int index = 0;
            string previousFolder = null;

            while (index < commonFolders.Count)
            {
                string folder = commonFolders[index];

                if (previousFolder != null && folder.StartsWith(previousFolder))
                    commonFolders.RemoveAt(index);
                else
                {
                    previousFolder = folder;
                    ++index;
                }
            }

            return commonFolders;
        }
    }
}
