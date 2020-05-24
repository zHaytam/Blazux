using Blazux.Core;

namespace WebCounterSample.Store
{
    public static class Reducers
    {
        [Reducer]
        public static State Handle(State state, IncrementCounterAction action)
        {
            return new State(state.CurrentCount + 1);
        }
    }
}
