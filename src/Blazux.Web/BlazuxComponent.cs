using Blazux.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace Blazux.Web
{
    public class BlazuxComponent<TState> : ComponentBase, IBlazuxComponent, IDisposable
    {

        [Inject]
        private IStore<TState> Store { get; set; }
        public TState State => Store.State;

        public T UseSelector<T>(Expression<Func<TState, T>> selectorExpr)
            => Store.UseSelector(selectorExpr, this);

        public T UseSelector<T, C>(Func<TState, T> selectorExpr, string fieldName) where C : BlazuxComponent<TState>
            => Store.UseSelector(selectorExpr, this, fieldName);

        public void Dispatch(IAction action) => Store.Dispatch(action);

        public virtual new void StateHasChanged() => base.StateHasChanged();

        public void Dispose() => Store.Unsubscribe(this);

    }
}
