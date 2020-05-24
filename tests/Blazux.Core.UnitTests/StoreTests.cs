using Moq;
using System;
using Xunit;

namespace Blazux.Core.UnitTests
{
    public class StoreTests
    {
        [Fact]
        public void UseSelector_ShouldRegisterSubscriptionAndReturnCorrectValue()
        {
            var store = new Store<State>(new State(2));
            var component = new Mock<IBlazuxComponent>().Object;

            int value = store.UseSelector(s => s.Count, component);

            Assert.Equal(store.State.Count, value);
        }

        [Fact]
        public void UseSelector_ShouldRegisterFieldSubscriptionAndReturnCorrectValue()
        {
            var store = new Store<State>(new State(8));
            var component = new Component();

            int value = store.UseSelector(s => s.Count, component, "_count");

            Assert.Equal(store.State.Count, value);
        }

        [Fact]
        public void Dispatch_ShouldChangeState_WhenReducerDoes()
        {
            var state = new State(5);
            var store = new Store<State>(state);
            var component = new Component();
            Func<State, Action, State> reducer = (s, a) => new State(s.Count + 1);

            store.AddReducer(reducer);
            store.Dispatch(new Action());

            Assert.NotEqual(state, store.State);
            Assert.Equal(6, store.State.Count);
        }

        [Fact]
        public void Dispatch_ShouldNotChangeState_WhenReducerDoesnt()
        {
            var state = new State(5);
            var store = new Store<State>(state);
            var component = new Component();
            var oldState = store.State;

            store.AddReducer<Action>((s, a) => s);
            store.Dispatch(new Action());

            Assert.Equal(state, store.State);
            Assert.Equal(5, store.State.Count);
        }

        [Fact]
        public void UseSelector_ShouldReturnCorrectValue_AfterStateChange()
        {
            var store = new Store<State>(new State(5));
            var component = new Component();
            Func<State, Action, State> reducer = (s, a) => new State(s.Count + 1);

            store.AddReducer(reducer);
            store.Dispatch(new Action());
            var value = store.UseSelector(s => s.Count, component);

            Assert.Equal(store.State.Count, value);
        }

        [Fact]
        public void Dispatch_ShouldUpdateField_WhenUseSelectorIsFieldBased()
        {
            var store = new Store<State>(new State(8));
            var component = new Component();

            int value = store.UseSelector(s => s.Count, component, "_count");
            store.AddReducer<Action>((s, a) => new State(s.Count + 1));
            store.Dispatch(new Action());

            Assert.Equal(9, component.Count);
        }

        [Fact]
        public void Dispatch_ShouldCallStateHasChangedOnlyOnce_WhenComponentSubscribesTwice()
        {
            var store = new Store<State>(new State(8));
            var componentMock = new Mock<IBlazuxComponent>();

            int value = store.UseSelector(s => s.Count, componentMock.Object);
            string otherValue = store.UseSelector(s => s.OtherValue, componentMock.Object);

            store.AddReducer<Action>((s, a) => new State(s.Count + 1, "test"));
            store.Dispatch(new Action());

            componentMock.Verify(c => c.StateHasChanged(), Times.Once());
        }

        [Fact]
        public void Dispatch_ShouldNotCallStateHasChanged_WhenComponentUnsuscribes()
        {
            var store = new Store<State>(new State(8));
            var componentMock = new Mock<IBlazuxComponent>();

            int value = store.UseSelector(s => s.Count, componentMock.Object);

            store.AddReducer<Action>((s, a) => new State(s.Count + 1, "test"));
            store.Unsubscribe(componentMock.Object);
            store.Dispatch(new Action());

            componentMock.Verify(c => c.StateHasChanged(), Times.Never());
        }
    }

    internal class Component : IBlazuxComponent
    {
        private int _count;
        public int Count => _count;

        public void StateHasChanged() { }
    }

    internal class Action : IAction
    {

    }

    internal class State
    {

        public int Count { get; set; }
        public string OtherValue { get; set; }

        public State(int count, string otherValue = null)
        {
            Count = count;
            OtherValue = otherValue;
        }
    }
}
