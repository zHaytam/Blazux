using Blazux.Core;

namespace PlainSample.Store
{
    public class IncrementWeatherAction : IAction
    {

        private static int _i = 0;

        public static State Reducer(State state, IncrementWeatherAction action)
        {
            var newState = state.ShallowClone();

            _i += 1;
            newState.CurrentWeather = new WeatherEntry
            {
                Country = $"Weather{_i}",
                Temperature = _i
            };

            return newState;
        }

    }
}
