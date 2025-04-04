﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.Attributes;

namespace Phase.Tests.Features
{
    public static class TableExtensions
    {
        public static T CreateImmutableInstance<T>(this Table table)
        {
            var instanceTable = GetTheProperInstanceTable(table, typeof(T));
            return ThisTypeHasADefaultConstructor<T>()
                       ? CreateTheInstanceWithTheDefaultConstructor<T>(instanceTable)
                       : CreateTheInstanceWithTheValuesFromTheTable<T>(instanceTable);
        }

        public static IEnumerable<T> CreateImmutableSet<T>(this Table table)
        {
            var rvalues = new List<T>();

            foreach (var row in table.Rows)
            {
                var tableInstance = new Table(table.Header.ToArray());
                tableInstance.AddRow(row.Values.ToArray());
                rvalues.Add(CreateImmutableInstance<T>(tableInstance));
            }

            return rvalues;
        }

        public static bool IsValueTupleType(Type type, bool checkBaseTypes = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(Tuple))
                return true;

            while (type != null)
            {
                if (type.IsGenericType)
                {
                    var genType = type.GetGenericTypeDefinition();
                    if
                    (
                        genType == typeof(ValueTuple)
                        || genType == typeof(ValueTuple<>)
                        || genType == typeof(ValueTuple<,>)
                        || genType == typeof(ValueTuple<,,>)
                        || genType == typeof(ValueTuple<,,,>)
                        || genType == typeof(ValueTuple<,,,,>)
                        || genType == typeof(ValueTuple<,,,,,>)
                        || genType == typeof(ValueTuple<,,,,,,>)
                        || genType == typeof(ValueTuple<,,,,,,,>)
                    )
                        return true;
                }

                if (!checkBaseTypes)
                    break;

                type = type.BaseType;
            }

            return false;
        }

        public static Table OverrideDefaultAttributes(this Table tableWithOverriddenAttributes, Table tableWithDefaultAttributes)
        {
            var mergedTable = new Table(tableWithDefaultAttributes.Header.ToArray());

            foreach (var defaultRow in tableWithDefaultAttributes.Rows)
            {
                mergedTable.AddRow(tableWithOverriddenAttributes.Rows.FirstOrDefault(row => row.Id() == defaultRow.Id()) ?? defaultRow);
            }

            return mergedTable;
        }

        internal static T CreateTheInstanceWithTheDefaultConstructor<T>(Table table)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        internal static T CreateTheInstanceWithTheValuesFromTheTable<T>(Table table)
        {
            var constructor = GetConstructorMatchingToColumnNames<T>(table);
            if (constructor == null)
                throw new MissingMethodException($"Unable to find a suitable constructor to create instance of {typeof(T).Name}");

            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, typeof(T));

            var constructorParameters = constructor.GetParameters();
            var parameterValues = new object[constructorParameters.Length];
            for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
            {
                var parameterName = constructorParameters[parameterIndex].Name;
                var member = (from m in membersThatNeedToBeSet
                              where string.Equals(m.MemberName, parameterName, StringComparison.OrdinalIgnoreCase)
                              select m).FirstOrDefault();
                if (member != null)
                    parameterValues[parameterIndex] = member.GetValue();
            }
            return (T)constructor.Invoke(parameterValues);
        }

        internal static ConstructorInfo GetConstructorMatchingToColumnNames<T>(Table table)
        {
            var projectedPropertyNames = from property in typeof(T).GetProperties()
                                         from row in table.Rows
                                         where IsMemberMatchingToColumnName(property, row.Id())
                                         select property.Name.ToLower(CultureInfo.InvariantCulture);

            return (from constructor in typeof(T).GetConstructors()
                    where !projectedPropertyNames.Except(
                        from parameter in constructor.GetParameters()
                        select parameter.Name.ToLower(CultureInfo.InvariantCulture)).Any()
                    select constructor).FirstOrDefault();
        }

        internal static IEnumerable<MemberHandler> GetMembersThatNeedToBeSet(Table table, Type type)
        {
            var properties = from property in type.GetProperties()
                             from row in table.Rows
                             where TheseTypesMatch(type, property.PropertyType, row)
                                   && (IsMemberMatchingToColumnName(property, row.Id())
                                   || IsMatchingAlias(property, row.Id()))
                             select new MemberHandler { Type = type, Row = row, MemberName = property.Name, PropertyType = property.PropertyType, Setter = (i, v) => property.SetValue(i, v, null) };

            var fields = from field in type.GetFields()
                         from row in table.Rows
                         where TheseTypesMatch(type, field.FieldType, row)
                               && (IsMemberMatchingToColumnName(field, row.Id()) ||
                                IsMatchingAlias(field, row.Id()))
                         select new MemberHandler { Type = type, Row = row, MemberName = field.Name, PropertyType = field.FieldType, Setter = (i, v) => field.SetValue(i, v) };

            var memberHandlers = new List<MemberHandler>();

            memberHandlers.AddRange(properties);
            memberHandlers.AddRange(fields);

            // tuple special case
            var fieldInfos = type.GetFields();
            if (IsValueTupleType(type))
            {
                if (fieldInfos.Length > 7)
                {
                    throw new Exception("You should just map to tuple with small objects, types with more than 7 properties are not currently supported");
                }

                if (fieldInfos.Length == table.RowCount)
                {
                    for (var index = 0; index < table.Rows.Count; index++)
                    {
                        var field = fieldInfos[index];
                        var row = table.Rows[index];

                        if (TheseTypesMatch(type, field.FieldType, row))
                        {
                            memberHandlers.Add(new MemberHandler
                            {
                                Type = type,
                                Row = row,
                                MemberName = field.Name,
                                PropertyType = field.FieldType,
                                Setter = (i, v) => field.SetValue(i, v)
                            });
                        }
                    }
                }
            }

            return memberHandlers;
        }

        internal static Table GetTheProperInstanceTable(Table table, Type type)
        {
            return ThisIsAVerticalTable(table, type)
                ? table
                : FlipThisHorizontalTableToAVerticalTable(table);
        }

        internal static bool IsMemberMatchingToColumnName(MemberInfo member, string columnName)
        {
            return member.Name.MatchesThisColumnName(columnName);
        }

        internal static void LoadInstanceWithKeyValuePairs(Table table, object instance)
        {
            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, instance.GetType());

            membersThatNeedToBeSet.ToList()
                                  .ForEach(x => x.Setter(instance, x.GetValue()));
        }

        internal static bool MatchesThisColumnName(this string propertyName, string columnName)
        {
            var normalizedColumnName = NormalizePropertyNameToMatchAgainstAColumnName(RemoveAllCharactersThatAreNotValidInAPropertyName(columnName));
            var normalizedPropertyName = NormalizePropertyNameToMatchAgainstAColumnName(propertyName);

            return normalizedPropertyName.Equals(normalizedColumnName, StringComparison.OrdinalIgnoreCase);
        }

        internal static string NormalizePropertyNameToMatchAgainstAColumnName(string name)
        {
            // we remove underscores, because they should be equivalent to spaces that were removed too from the column names
            return name.Replace("_", string.Empty);
        }

        internal static string RemoveAllCharactersThatAreNotValidInAPropertyName(string name)
        {
            //Unicode groups allowed: Lu, Ll, Lt, Lm, Lo, Nl or Nd see https://msdn.microsoft.com/en-us/library/aa664670%28v=vs.71%29.aspx
            return new Regex(@"[^\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}_]").Replace(name, string.Empty);
        }

        internal static bool ThisTypeHasADefaultConstructor<T>()
        {
            return typeof(T).GetConstructors().Any(c => c.GetParameters().Length == 0);
        }

        private static Table FlipThisHorizontalTableToAVerticalTable(Table table)
        {
            return new PivotTable(table).GetInstanceTable(0);
        }

        private static bool IsMatchingAlias(MemberInfo field, string id)
        {
            var aliases = field.GetCustomAttributes().OfType<TableAliasesAttribute>();
            return aliases.Any(a => a.Aliases.Any(al => Regex.Match(id, al).Success));
        }

        private static bool TheFirstRowValueIsTheNameOfAProperty(Table table, Type type)
        {
            var firstRowValue = table.Rows[0][table.Header.First()];
            return type.GetProperties()
                       .Any(property => IsMemberMatchingToColumnName(property, firstRowValue));
        }

        private static bool TheHeaderIsTheOldFieldValuePair(Table table)
        {
            return table.Header.Count == 2 && table.Header.First() == "Field" && table.Header.Last() == "Value";
        }

        private static bool TheseTypesMatch(Type targetType, Type memberType, TableRow row)
        {
            return Service.Instance.GetValueRetrieverFor(row, targetType, memberType) != null;
        }

        private static bool ThisIsAVerticalTable(Table table, Type type)
        {
            if (TheHeaderIsTheOldFieldValuePair(table))
                return true;
            return (table.Rows.Count != 1) || (table.Header.Count == 2 && TheFirstRowValueIsTheNameOfAProperty(table, type));
        }

        internal class MemberHandler
        {
            public string MemberName { get; set; }

            public Type PropertyType { get; set; }

            public TableRow Row { get; set; }

            public Action<object, object> Setter { get; set; }

            public Type Type { get; set; }

            public object GetValue()
            {
                var valueRetriever = Service.Instance.GetValueRetrieverFor(Row, Type, PropertyType);
                return valueRetriever.Retrieve(new KeyValuePair<string, string>(Row[0], Row[1]), Type, PropertyType);
            }
        }

        internal class PivotTable
        {
            private readonly Table table;

            public PivotTable(Table table)
            {
                this.table = table;
            }

            public Table GetInstanceTable(int index)
            {
                AssertThatThisItemExistsInTheSet(index);

                return CreateAnInstanceTableForThisItemInTheSet(index);
            }

            private void AssertThatThisItemExistsInTheSet(int index)
            {
                if (table.Rows.Count <= index)
                    throw new IndexOutOfRangeException();
            }

            private Table CreateAnInstanceTableForThisItemInTheSet(int index)
            {
                var instanceTable = new Table("Field", "Value");
                foreach (var header in table.Header)
                    instanceTable.AddRow(header, table.Rows[index][header]);
                return instanceTable;
            }
        }
    }
}