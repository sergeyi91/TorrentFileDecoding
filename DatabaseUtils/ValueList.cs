using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DatabaseUtils
{
   /// <summary>
   /// List of collected ValueType values
   /// </summary>
   /// <typeparam name="ValueType">either some structure, class or primitive type. (see ValuesCollector class)</typeparam>
   public class ValueList<ValueType>: IEnumerable<ValueType>
   {
      /// <summary>
      /// constructs the list with checking that ValueType is acceptable for collection.
      /// only primitive types and structures with primitive fields can be used
      /// </summary>
      public ValueList()
      {
         //check that the provided type is acceptable for collecting
         Type valType = typeof(ValueType);
         if (!IsPrimitiveType(valType) &&
              !valType.IsValueType && !valType.IsClass
             )
         {
            throw new Exception("Only primitive types, classes and structs are acceptable for ValuesCollector, " +
                valType.FullName + " is not acceptable");
         }
      }

      public ValueList(bool extractProperties) : this()
      {
         this.extractProperties = extractProperties;
      }

      /// <summary>
      /// include the value to the collection
      /// </summary>
      /// <param name="value">value to include into the set</param>
      public void OnValue(ValueType value)
      {
         m_valuesList.Add(value);
      }

      /// <summary>
      /// get collected values as a result table with tableName name
      /// </summary>
      /// <param name="tableName">table name</param>
      /// <returns>collected results table where a column is a ValueType structure field</returns>
      public DataTable GetCollectedData(string tableName)
      {
         DataTable valueTable = new DataTable(tableName);
         addTableColumns(valueTable);

         Type valType = typeof(ValueType);

         bool isPrimitiveType = IsPrimitiveType(valType);
         MemberInfo[] memberInfos = valType.GetMembers();

         foreach (ValueType tv in m_valuesList)
         {
            if (isPrimitiveType)
            {
               addTableRowForPrimitiveType(valueTable, tv);
            }
            else
            {
               addTableRowForNonPrimitiveType(valueTable, tv, memberInfos);
            }
         }

         return valueTable;
      }

      public void Clear()
      {
         m_valuesList.Clear();
      }

      private static bool IsPrimitiveType(Type tp)
      {
         return tp.IsPrimitive || tp == typeof(string) || tp == typeof(DateTime) || tp.IsEnum || tp.Name.Contains("Nullable");
      }

      private Type getTypeFromMemberInfo(MemberInfo memberInfo)
      {
         if (memberInfo.MemberType == MemberTypes.Field)
         {
            return ((FieldInfo)memberInfo).FieldType;
         }
         if (extractProperties && memberInfo.MemberType == MemberTypes.Property)
         {
            return ((PropertyInfo)memberInfo).PropertyType;
         }
         return null;
      }

      private object getValueFromMemberInfo(MemberInfo member, object value)
      {
         if (member.MemberType == MemberTypes.Field)
         {
            return ((FieldInfo)member).GetValue(value);
         }
         if (extractProperties && member.MemberType == MemberTypes.Property)
         {
            return ((PropertyInfo)member).GetValue(value, null);
         }
         return null;
      }

      private void addTableColumns(DataTable valueTable)
      {
         Type valType = typeof(ValueType);
         if (IsPrimitiveType(valType))
         {
            //add as a single Value field
            valueTable.Columns.Add("Value", valType);
         }
         else
         {
            //structure or class, add primitive fields
            var memberInfos = valType.GetMembers();
            foreach (MemberInfo memberInfo in memberInfos)
            {
               Type type = getTypeFromMemberInfo(memberInfo);
               if (type == null)
               {
                  continue;
               }

               //only primitive types are saved
               if (IsPrimitiveType(type))
               {
                  // Workarround for nullable numeric fields
                  if (type.Name.Contains("Nullable"))
                  {
                     // E.g. FullName = "System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"
                     int idxStart = type.FullName.IndexOf("[System.") + 1;
                     int idxEnd = type.FullName.IndexOf(", mscorlib", idxStart);
                     string typeName = type.FullName.Substring(idxStart, idxEnd - idxStart);
                     type = Type.GetType(typeName);
                  }

                  DataColumn createdColumn = valueTable.Columns.Add(memberInfo.Name, type);
                  MaxStringLengthAttribute atr =
                      (MaxStringLengthAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(MaxStringLengthAttribute));
                  if (atr != null)
                  {
                     createdColumn.MaxLength = atr.MaxLength;
                  }
               }
            }
         }

      }

      private static void addTableRowForPrimitiveType(DataTable valueTable, ValueType value)
      {
         object[] tableFields = { value };
         valueTable.Rows.Add(tableFields);
      }

      private void addTableRowForNonPrimitiveType(DataTable valueTable, ValueType value, MemberInfo[] membersList)
      {
         List<object> tableFields = new List<object>();

         //structure, add members
         foreach (MemberInfo member in membersList)
         {
            Type type = getTypeFromMemberInfo(member);
            if (type == null)
            {
               continue;
            }
            if (IsPrimitiveType(type))
            {
               tableFields.Add(getValueFromMemberInfo(member, value));
            }
         }
         valueTable.Rows.Add(tableFields.ToArray());
      }

      private readonly List<ValueType> m_valuesList =
          new List<ValueType>();

      public int Count
      {
         get { return m_valuesList.Count; }
      }

      public IEnumerator<ValueType> GetEnumerator()
      {
         return m_valuesList.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return this.GetEnumerator();
      }

      /// <summary>
      /// If false - only fields will be extracted.
      /// If true - properties will be also extracted.
      /// </summary>
      private readonly bool extractProperties;
   }
}
