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

using System;
using System.Collections;
using System.Collections.Generic;

namespace VSPackage_UnitTests
{
    static class PropertyHelper
    {
        //---------------------------------------------------------------------
        public static void SetPropertiesValue<T>(T value, Dictionary<Type, Func<object>> factoriesByType)
        {
            var properties = value.GetType().GetProperties();

            foreach (var p in properties)
            {
                if (p.CanWrite)
                {
                    Func<object> factory;
                    if (factoriesByType.TryGetValue(p.PropertyType, out factory))
                        p.SetValue(value, factory());
                    else
                        throw new NotSupportedException($"Type {p.PropertyType} is not supported.");
                }
            }
        }

        //---------------------------------------------------------------------
        public static void CheckPropertiesEqualRecursive<T>(T value1, T value2)
        {
            CheckPropertiesEqualRecursive(value1, value2, typeof(T).Name);
        }

        //---------------------------------------------------------------------
        static void CheckPropertiesEqualRecursive(object value1, object value2, string name)
        {
            name += "/";
            System.Diagnostics.Debug.WriteLine(name);

            var isValue1Null = value1 == null;
            var isValue2Null = value2 == null;

            if (isValue1Null != isValue2Null)
                throw new Exception($"{name} mismatch : {value1} VS {value2}");
            if (value1 == null)
                return; // both values are null

            var type1 = value1.GetType();
            var type2 = value2.GetType();

            if (type1 != type2)
                throw new Exception($"{name} type mismatch : {type1} VS {type2}");

            if (type1.IsPrimitive || type1 == typeof(string))
            {
                if (!value1.Equals(value2))
                    throw new Exception($"{name} mismatch: {value1} VS {value2}");
            }
            else if (value1 is IEnumerable)
                CheckPropertiesEqualRecursiveEnumerable(value1, value2, name);
            else
                CheckPropertiesEqualRecursiveObject(value1, value2, name);
        }

        //---------------------------------------------------------------------
        static void CheckPropertiesEqualRecursiveEnumerable(object value1, object value2, string name)
        {
            var enumerator1 = ((IEnumerable)value1).GetEnumerator();
            var enumerator2 = ((IEnumerable)value2).GetEnumerator();

            while (true)
            {
                var next1 = enumerator1.MoveNext();

                if (next1 != enumerator2.MoveNext())
                    throw new Exception($"{name} has not the same number of elements.");

                if (!next1)
                    break;
                CheckPropertiesEqualRecursive(enumerator1.Current, enumerator2.Current, name + "items");
            }
        }

        //---------------------------------------------------------------------
        static void CheckPropertiesEqualRecursiveObject(object value1, object value2, string name)
        {
            var properties = value1.GetType().GetProperties();

            foreach (var p in properties)
            {
                var type = p.PropertyType;
                var v1 = p.GetValue(value1);
                var v2 = p.GetValue(value2);

                CheckPropertiesEqualRecursive(v1, v2, name + p.Name);
            }
        }
    }
}
