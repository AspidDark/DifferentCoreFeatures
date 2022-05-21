namespace DIFeatures
{
    //+Imp1
    public interface IHelloer
    {
        string CurrentName { get; }
        string SayHello();
    }

    public class HelloerA : IHelloer
    {
        public string CurrentName => nameof(HelloerA);

        public string SayHello()
        {
            return $"Hello from {nameof(HelloerA)}";
        }
    }

    public class HelloerB : IHelloer
    {
        public string CurrentName => nameof(HelloerB);

        public string SayHello()
        {
            return $"Hello from {nameof(HelloerB)}";
        }
    }
    //-Imp1
}
