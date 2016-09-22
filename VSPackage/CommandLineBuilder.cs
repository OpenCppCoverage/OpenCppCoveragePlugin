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

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCppCoverage.VSPackage
{
    class CommandLineBuilder
    {
        readonly List<string> commandLines = new List<string>();
        
        //---------------------------------------------------------------------
        public string GetCommandLine(string separator = " ")
        {
            return String.Join(separator, this.commandLines);
        }

        //---------------------------------------------------------------------
        public CommandLineBuilder AppendArgumentCollection(
                        string argumentName, IEnumerable<string> values)
        {
            foreach (var value in values)
                AppendArgument(argumentName, value);
            return this;
        }

        //---------------------------------------------------------------------
        public CommandLineBuilder AppendArgument(
            string argumentName,
            string optionalArgumentValue)
        {
            var line = argumentName;
            if (optionalArgumentValue != null)
                line += ' ' + EscapeValue(optionalArgumentValue);
            commandLines.Add(line);
            return this;
        }

        //---------------------------------------------------------------------
        public CommandLineBuilder Append(string str)
        {
            commandLines.Add(str);
            return this;
        }

        //---------------------------------------------------------------------
        static public string EscapeValue(string str)
        {
            var builder = new StringBuilder();
            int consecutiveBackSlashCount = 0;

            builder.Append('\"');
            foreach (var c in str)
            {
                if (c != '\"')
                    builder.Append(c);
                else
                {
                    // Back slash before quote need to be escaped
                    // so add the same number of back slash. 
                    builder.Append('\\', consecutiveBackSlashCount);

                    // Quote need to be escaped too.
                    builder.Append(@"\""");
                }
                consecutiveBackSlashCount = (c == '\\') ? consecutiveBackSlashCount + 1 : 0;
            }

            // We need to escape last slash because we add '"' just after
            builder.Append('\\', consecutiveBackSlashCount);
            builder.Append('\"');

            return builder.ToString();
        }
    }   
}