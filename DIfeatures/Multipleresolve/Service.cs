namespace DIFeatures
{
    public interface IContractService {
        public int MyProperty { get; set; }
    }


    public class Service1 : IContractService
    {
        public int MyProperty { get; set; } = 1;
    }

    public class Service2 : IContractService
    {
        public int MyProperty { get; set; } = 2;
    }
}
