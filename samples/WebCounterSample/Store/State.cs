namespace WebCounterSample.Store
{
    public class State
    {

        public int CurrentCount { get; }

        public State(int count)
        {
            CurrentCount = count;
        }
    }
}
