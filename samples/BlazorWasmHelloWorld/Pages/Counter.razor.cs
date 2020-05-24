using Blazux.Core;
using Microsoft.AspNetCore.Components;
using PlainSample.Store;
using System;

namespace PlainSample.Pages
{
    public class CounterComponent : ComponentBase, IBlazuxComponent, IDisposable
    {
        [Inject] 
        private IStore<State> Store { get; set; }
        protected int CurrentCount => Store.UseSelector(s => s.CurrentCount, this);

        public void IncrementCount() => Store.Dispatch(new IncrementCounterAction(10));

        public void Dispose() => Store.OnComponentDisposed(this);

        public new void StateHasChanged() => base.StateHasChanged();
    }
}
