using Blazux.Web;
using WebCounterSample.Store;

namespace WebCounterSample.Pages
{
    public class CounterComponent : BlazuxComponent<State>
    {
        protected int CurrentCount => UseSelector(s => s.CurrentCount);

        protected void Increment() => Dispatch(new IncrementCounterAction());
    }
}
