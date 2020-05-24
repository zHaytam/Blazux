using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Blazux.Core
{
    public static class Extensions
    {
        private static readonly Type _reducerAttributeType = typeof(ReducerAttribute);

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

        public static bool IsReducer<TState>(this MethodInfo method, out Type actionType)
        {
            actionType = null;

            if (method.GetCustomAttribute<ReducerAttribute>() == null)
                return false;

            if (method.ReturnType != typeof(TState))
                throw new Exception($"Reducer '{method.Name}' should return a state.");

            var parameters = method.GetParameters();
            if (parameters.Length != 2)
                throw new Exception($"Reducer '{method.Name}' must have 2 parameters.");

            if (parameters[0].ParameterType != typeof(TState))
                throw new Exception($"The first parameter of reducer '{method.Name}' must be of type TState.");

            if (!typeof(IAction).IsAssignableFrom(parameters[1].ParameterType))
                throw new Exception($"The second parameter of reducer '{method.Name}' must implement IAction.");

            actionType = parameters[1].ParameterType;
            return true;
        }

        public static Func<TState, IAction, TState> GetReducerFunc<TState>(this MethodInfo method, Type actionType)
        {
            var stateParam = Expression.Parameter(typeof(TState), "state");
            var actionParam = Expression.Parameter(typeof(IAction), "action");
            var callExpr = Expression.Call(method, stateParam, Expression.Convert(actionParam, actionType));
            var lambdaExpr = Expression.Lambda<Func<TState, IAction, TState>>(callExpr, stateParam, actionParam);
            return lambdaExpr.Compile();
        }
    }
}
