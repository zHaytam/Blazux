using System;

namespace Blazux.Core
{
    internal class Subscription<TState>
    {

        public Func<TState, object> Selector { get; set; }
        public object LatestValue { get; set; }
        public Action<object, object> FieldSetter { get; set; }

    }
}
