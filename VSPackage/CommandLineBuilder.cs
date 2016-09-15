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
using System.Text;

namespace OpenCppCoverage.VSPackage
{
    class CommandLineBuilder
    {
        readonly StringBuilder builder = new StringBuilder();

        //---------------------------------------------------------------------
        public string CommandLine { get { return this.builder.ToString(); } }
        
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
            if (this.builder.Length != 0)
                this.builder.Append(' ');

            this.builder.Append(argumentName);
            if (optionalArgumentValue != null)
            {
                this.builder.Append(' ');
                this.builder.Append(EscapeValue(optionalArgumentValue));
            }
            return this;
        }

        //---------------------------------------------------------------------
        public CommandLineBuilder Append(string str)
        {
            this.builder.Append(str);
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