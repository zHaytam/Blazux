using Blazux.Core;
using System;
using System.Reflection;

namespace Blazux.Web
{
    public class StoreBuilder<TState>
    {

        internal Store<TState> Store { get; }

        internal StoreBuilder(TState initialState = default)
        {
            Store = new Store<TState>(initialState);
        }

        public void AddReducer<TAction>(Func<TState, TAction, TState> reducer) where TAction : IAction
            => Store.AddReducer(reducer);

        public void AddReducersFromAssembly(Assembly assembly, params Type[] typesToExclude)
            => Store.AddReducersFromAssembly(assembly, typesToExclude);

    }
}
