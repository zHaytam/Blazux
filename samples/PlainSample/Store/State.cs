namespace PlainSample.Store
{
    public class State
    {

        public int CurrentCount { get; set; }
        public WeatherEntry CurrentWeather { get; set; }

        public State ShallowClone() => (State)MemberwiseClone();

    }

    public class WeatherEntry
    {

        public string Country { get; set; }
        public double Temperature { get; set; }

    }
}
