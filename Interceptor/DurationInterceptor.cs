using Castle.DynamicProxy;
using System.Diagnostics;

namespace InterceptPipeline
{
    public class DurationInterceptor : IInterceptor
    {
        //Pipeline Like
        public void Intercept(IInvocation invocation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                //Before
                invocation.Proceed();
                //after
            }
            finally 
            {
                sw.Stop();
                var methodName=invocation.Method.Name;
                var result= sw.Elapsed;

            }
        }
    }
}
