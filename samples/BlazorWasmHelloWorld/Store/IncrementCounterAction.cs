using Blazux.Core;

namespace PlainSample.Store
{
    public class IncrementCounterAction : IAction
    {

        public int Amount { get; }

        public IncrementCounterAction(int amount)
        {
            Amount = amount;
        }

        public static State Reducer(State state, IncrementCounterAction action)
        {
            var newState = state.ShallowClone();
            newState.CurrentCount += action.Amount;
            return newState;
        }

    }
}
