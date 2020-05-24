using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Blazux.Core
{
    public static class Extensions
    {
        public static Action<T, object> BuildFieldSetter<T>(this FieldInfo field)
        {
            if (field.IsInitOnly)
                throw new Exception("Cannot build setter for a read only field.");

            var targetType = field.DeclaringType;
            var exInstance = Expression.Parameter(targetType, "t");

            var exMemberAccess = Expression.MakeMemberAccess(exInstance, field);

            var exValue = Expression.Parameter(typeof(object), "p");
            var exConvertedValue = Expression.Convert(exValue, field.FieldType);
            var exBody = Expression.Assign(exMemberAccess, exConvertedValue);

            var lambda = Expression.Lambda<Action<T, object>>(exBody, exInstance, exValue);
            return lambda.Compile();
        }

    }
}
