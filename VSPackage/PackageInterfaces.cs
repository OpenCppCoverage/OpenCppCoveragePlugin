// OpenCppCoverage is an open source code coverage for C++.
// Copyright (C) 2019 OpenCppCoverage
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

using Microsoft.VisualStudio.Shell;
using System;

namespace OpenCppCoverage.VSPackage
{
    /// <summary>
    /// PackageInterfaces implements the interfaces for OpenCppCoveragePackage.
    /// 
    /// OpenCppCoveragePackage cannot implement the interface directly, otherwise
    /// it would be necessary to add a reference to all interfaces (Some of them are from
    /// VS 15)
    /// Note: We use a lambda for GetService because Package.GetService is protected.
    /// </summary>
    class PackageInterfaces : IServiceProvider, IWindowFinder
    {        
        readonly Package package;
        readonly Func<Type, object> getService;

        //---------------------------------------------------------------------
        public PackageInterfaces(Package package, Func<Type, object> getService)
        {
            this.package = package;
            this.getService = getService;
        }

        //---------------------------------------------------------------------
        public T FindToolWindow<T>() where T : ToolWindowPane
        {
            var type = typeof(T);
            var window = this.package.FindToolWindow(type, 0, true) as T;
            if (window == null || window.Frame == null)
                throw new NotSupportedException("Cannot create window " + type.Name);
            return window;
        }

        //---------------------------------------------------------------------
        public object GetService(Type serviceType)
        {
            return this.getService(serviceType);
        }
    }
}
