using AOPDemo.Services.Students;

namespace AOPDemo
{
    //https://www.youtube.com/watch?v=KOmsEKgSl08
    public class Program
    {
        static void Main(string[] args)
        {
            IStudentService studentService =
                new StudentService();

            studentService = StudentServiceDispatch<IStudentService>
                .Create(studentService);

            studentService.AddStudent();
        }
    }
}