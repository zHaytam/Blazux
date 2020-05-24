using System;
using System.Linq.Expressions;

namespace Blazux.Core
{
    public interface IStore<TState>
    {

        public TState State { get; }

        public T UseSelector<T>(Expression<Func<TState, T>> selectorExpr, IBlazuxComponent component);

        public T UseSelector<T, C>(Func<TState, T> selector, C component, string fieldName) where C : IBlazuxComponent;

        public void AddReducer<TAction>(Func<TState, TAction, TState> func) where TAction : IAction;

        public void Dispatch(IAction action);

        public void Unsubscribe(IBlazuxComponent component);

    }
}
