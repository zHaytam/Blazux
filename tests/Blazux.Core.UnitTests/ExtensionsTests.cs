using System;
using System.Reflection;
using Xunit;

namespace Blazux.Core.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void BuildFieldSetter_ShouldThrow_WhenFieldIsReadonly()
        {
            var field = typeof(Entry).GetField("_otherId", BindingFlags.NonPublic | BindingFlags.Instance);

            var ex = Assert.Throws<Exception>(() => field.BuildFieldSetter<Entry>());
            Assert.Equal("Cannot build setter for a read only field.", ex.Message);
        }

        [Fact]
        public void BuildFieldSetter_ShouldReturnWorkingSetter()
        {
            var field = typeof(Entry).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
            var setter = field.BuildFieldSetter<Entry>();

            var entry = new Entry();
            setter(entry, 1000);

            Assert.Equal(1000, entry.Id);
        }

        [Fact]
        public void IsReducer_ShouldReturnFalse_WhenMethodDoesntHaveAttribute()
        {
            var method = typeof(ExtensionsReducers)
                .GetMethod(nameof(ExtensionsReducers.WithoutAttribute), BindingFlags.Static | BindingFlags.Public);

            Assert.False(method.IsReducer<int>(out _));
        }

        [Theory]
        [InlineData(nameof(ExtensionsReducers.WrongReturnType), "Reducer 'WrongReturnType' should return a state.")]
        [InlineData(nameof(ExtensionsReducers.MoreThanTwoArguments), "Reducer 'MoreThanTwoArguments' must have 2 parameters.")]
        [InlineData(nameof(ExtensionsReducers.WrongStateType), "The first parameter of reducer 'WrongStateType' must be of type TState.")]
        [InlineData(nameof(ExtensionsReducers.WrongActionType), "The second parameter of reducer 'WrongActionType' must implement IAction.")]
        public void IsReducer_ShouldThrow_WhenSomethingIsWrong(string methodName, string exceptionMsg)
        {
            var method = typeof(ExtensionsReducers).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            var ex = Assert.Throws<Exception>(() => method.IsReducer<int>(out _));
            Assert.Equal(exceptionMsg, ex.Message);
        }


        [Fact]
        public void IsReducer_ShouldReturnTrue_WhenMethodIsValid()
        {
            var method = typeof(ExtensionsReducers)
                .GetMethod(nameof(ExtensionsReducers.Valid), BindingFlags.Static | BindingFlags.Public);

            Assert.True(method.IsReducer<int>(out var actionType));
            Assert.Equal(typeof(IAction), actionType);
        }

        [Fact]
        public void GetReducerFunc_ShouldReturnWorkingFunc()
        {
            var method = typeof(ExtensionsReducers)
                .GetMethod(nameof(ExtensionsReducers.Valid), BindingFlags.Static | BindingFlags.Public);

            var func = method.GetReducerFunc<int>(typeof(IAction));
            Assert.Equal(10, func(10, null));
        }
    }

    static class ExtensionsReducers
    {
        public static void WithoutAttribute() { }

        [Reducer]
        public static void WrongReturnType(int state, IAction action) { }

        [Reducer]
        public static int MoreThanTwoArguments(int state, IAction action, object something) => state;

        [Reducer]
        public static int WrongStateType(string state, IAction action) => 0;

        [Reducer]
        public static int WrongActionType(int state, string action) => state;

        [Reducer]
        public static int Valid(int state, IAction action) => state;
    }

    class Entry
    {
        private int _id;
        private readonly int _otherId;

        public int Id => _id;
    }
}
