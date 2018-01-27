using System;

namespace DatabaseUtils
{
   // A field attribute that allows configuring maximal length of string field while persisting it to DB.
   // In other words it specifies the size of varchar DB type, which is 'max' by default.
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
   public class MaxStringLengthAttribute : Attribute
   {
      public MaxStringLengthAttribute(int maxLength) { MaxLength = maxLength; }

      public int MaxLength { get; set; }
   }
}
