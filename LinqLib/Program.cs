using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LinqLib.Additional;

namespace LinqLib
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] cars = { "Nissan", "Aston Martin", "Chevrolet", "Alfa Romeo", "Chrysler", "Dodge", "BMW",
       "Ferrari", "Audi", "Bentley", "Ford", "Lexus", "Mercedes", "Toyota", "Volvo", "Subaru", "Жигули :)"};
            #region Where используется для фильтрации элементов в последовательность

            IEnumerable<string> sequence = cars.Where(p => p.StartsWith("F"));
            IEnumerable<string> sequence2 = cars.Where((p, i) => (i & 1) == 1);
            #endregion

            #region Select и SelectMany используется для создания выходной последовательности одного типа элементов из входной последовательности элементов другого типа
            IEnumerable<int> sequence3 = cars.Select(p => p.Length);
            var carObj = cars.Select(p => new { LastName = p, Length = p.Length });
            IEnumerable<char> chars = cars.SelectMany(p => p.ToArray());
            #endregion

            #region Take, TakeWhile, Skip и SkipWhile
            //Операция Take возвращает указанное количество элементов из входной последовательности, начиная с ее начала
            IEnumerable<string> auto = cars.Take(5); //первые 5
            //Операция TakeWhile возвращает элементы из входной последовательности, пока истинно некоторое условие, начиная с начала последовательности
            IEnumerable<string> auto2 = cars.TakeWhile(s => s.Length < 12);
            IEnumerable<string> auto3 = cars.TakeWhile((s, i) => s.Length < 12 && i < 5);

            //Операция Skip пропускает указанное количество элементов из входной последовательности, начиная с ее начала, и выводит остальные
            IEnumerable<string> auto4 = cars.Skip(5);

            //Операция SkipWhile обрабатывает входную последовательность, пропуская элементы до тех пор, пока условие истинно, а затем выводит остальные в выходную последовательность
            IEnumerable<string> auto5 = cars.SkipWhile(s => s.StartsWith("A"));
            IEnumerable<string> auto6 = cars.SkipWhile((s, i) => s.StartsWith("A") && i < 10);
            #endregion

            #region Concat соединяет две входные последовательности и выдает одну выходную последовательность
            IEnumerable<string> auto7 = cars.Take(6).Concat(cars.Skip(4)).OrderBy(x=>x);
            #endregion

            #region OrderBy и OrderByDescending Операции упорядочивания позволяют выстраивать входные последовательности в определенном порядке
            //Если требуется большая степень упорядочивания, чем возможно достичь с помощью одиночного вызова операции OrderBy или OrderByDescending, необходимо последовательно вызывать операции ThenBy или ThenByDescending,
            IEnumerable<string> auto8 = cars.OrderBy(s => s.Length);
            MyVowelToConsonantRatioComparer myComp = new MyVowelToConsonantRatioComparer();
            IEnumerable<string> auto9 = cars.OrderBy((s => s), myComp); //С созданным компаратором


            IEnumerable<string> auto10 = cars.OrderByDescending(s => s);
            #endregion

            #region Операции ThenBy и ThenByDescending
            //Операция ThenBy позволяет упорядочивать входную последовательность типа IOrderedEnumerable<T> на основе метода keySelector, который возвращает значение ключа.
            IEnumerable<string> auto11 = cars.OrderBy(s => s.Length).ThenBy(s => s);


            IEnumerable<string> auto12 = cars
                .OrderBy(s => s.Length)
                .ThenBy((s => s), myComp);


            IEnumerable<string> auto13 = cars.OrderBy(s => s.Length).ThenByDescending(s => s);
            #endregion

            Employee[] employees = Employee.GetEmployeesArray();
            EmployeeOptionEntry[] empOptions = EmployeeOptionEntry.GetEmployeeOptionEntries();
            #region Операции Join и GroupJoin
            //Операция Join выполняет внутреннее соединение по эквивалентности двух последовательностей на основе ключей, извлеченных из каждого элемента этих последовательностей

            var employeeOptions = employees
              .Join(
                empOptions,       //  inner sequence
                e1 => e1.id,        //  outerKeySelector
                o => o.id,        //  innerKeySelector
                (e1, o) => new       //  resultSelector
          {
                    id = e1.id,
                    name = string.Format("{0} {1}", e1.firstName, e1.lastName),
                    options = o.optionsCount
                });

            //Операция GroupJoin выполняет групповое соединение двух последовательностей на основе ключей, извлеченных из каждого элемента последовательностей
            var employeeOptions2 = employees
        .GroupJoin(
          empOptions,
          e1 => e1.id,
          o => o.id,
          (e1, os) => new
          {
              id = e1.id,
              name = string.Format("{0} {1}", e1.firstName, e1.lastName),
              options = os.Sum(o => o.optionsCount)
          });
            #endregion

            #region Операция GroupBy

            IEnumerable<IGrouping<int, EmployeeOptionEntry>> outerSequence =
              empOptions.GroupBy(o => o.id);
            #endregion
            int[] arr = { 1, 10, 5, 8, 5, 1, 12, 5, 9, 9, 2 };
            int[] arr2 = { 6, 7, 8, 9 };
            #region Операции Distinct, Union, Except и Intersect  Операции множеств используются для выполнения математических операций с множествами на последовательностях
            //Distinct Эта операция возвращает объект, перечисляющий элементы входной последовательности source и выдающий последовательность, в которой каждый элемент не эквивалентен предыдущим выданным
            IEnumerable<int> nums = arr.Distinct();
            //Операция Union возвращает объединение множеств из двух исходных последовательностей.
            IEnumerable<string> first = cars.Take(5);
            IEnumerable<string> second = cars.Skip(4);
            IEnumerable<string> union = first.Union<string>(second);
            //Операция Intersect возвращает пересечение множеств из двух исходных последовательностей.
            IEnumerable<string> auto14 = first.Intersect(second);

            //Операция Except возвращает последовательность, содержащую все элементы первой последовательности, которых нет во второй последовательности
            IEnumerable<int> nums1 = arr.Except<int>(arr2);
            #endregion

            ArrayList al = new ArrayList();
            al.Add(new Employee { id = 1, firstName = "Joe", lastName = "Rattz" });
            al.Add(new Employee { id = 2, firstName = "William", lastName = "Gates" });
            al.Add(new EmployeeOptionEntry { id = 1, optionsCount = 0 });
            al.Add(new EmployeeOptionEntry { id = 2, optionsCount = 99999999999 });
            al.Add(new Employee { id = 3, firstName = "Anders", lastName = "Hejlsberg" });
            al.Add(new EmployeeOptionEntry { id = 3, optionsCount = 848475745 });
            #region Операции Cast, OfType и AsEnumerable Операции преобразования предоставляют простой и удобный способ преобразования последовательностей в другие типы коллекций
            var seq = employees.Cast<Employee>();

            //Мораль сей басни в том, что если входная последовательность может содержать элементы более чем одного типа данных, следует отдать предпочтение операции OfType перед Cast.
            var items2 = al.OfType<Employee>();
            #endregion


            #region DefaultIfEmpty Операции элементов позволяют извлекать элементы из входной последовательности.
            string nissan = cars.Where(n => n.Equals("Porshe")).DefaultIfEmpty().First();

            string porshe = cars
        .Where(n => n.Equals("Porshe"))
        .DefaultIfEmpty("Не найдено :(")
        .First();
            #endregion

            #region Операции Range, Repeat и Empty
            //Операция Range генерирует последовательность целых чисел
            IEnumerable<int> nums5 = Enumerable.Range(1, 100);
            //Операция Repeat генерирует последовательность, повторяя указанный элемент заданное количество раз.
            IEnumerable<int> nums6 = Enumerable.Repeat(2, 10);
            //Операция Empty генерирует пустую последовательность заданного типа.
            IEnumerable<string> str = Enumerable.Empty<string>();
            #endregion
            //_____________________________________________________________ 
            #region Операции ToArray и ToList
            //Операция ToArray создает массив типа T из входной последовательности типа T
            IEnumerable<int> item2 = Enumerable.Range(1, 20);
            int[] arr6 = item2.ToArray();
            //Операция ToList создает List типа T из входной последовательности типа Т
            List<string> auto15 = cars.ToList();
            #endregion

            #region Операция ToDictionary
            //Операция ToDictionary создает Dictionary типа <К,Т>, или, возможно, 
            //<К, Е>, если прототип имеет аргумент elementSelector, из входной последовательности типа Т,
            //где К — тип ключа, а T — тип хранимых значений. Или же, если Dictionary имеет тип <К, Е>,
            //то типом хранимых значений будет Е, отличающийся от типа элементов в последовательности — Т
            Dictionary<int, Employee> eDictionary = Employee.GetEmployeesArray().ToDictionary(k => k.id);


            Dictionary<int, Employee> eDictionary3 =
      Employee.GetEmployeesArray().ToDictionary(k => k.id);
            Employee e = eDictionary[2];
            Dictionary<string, Employee2> e2Dictionary3 =
                  Employee2.GetEmployeesArray().ToDictionary(k => k.id, new MyStringifiedNumberComparer());
            Employee2 e2 = e2Dictionary3["2"];

            Dictionary<int, string> eDictionary4 =
     Employee.GetEmployeesArray().ToDictionary(k => k.id,
                                               i => string.Format("{0} {1}",
                                                                 i.firstName, i.lastName));


            var eDictionary5 =
                Employee2.GetEmployeesArray().ToDictionary(k => k.id,
                                                          i => string.Format("{0} {1}",
                                                              i.firstName, i.lastName),
                                                          new MyStringifiedNumberComparer());
            #endregion

            #region Операция ToLookup  много значений с 1 ключем!!!
            //Операция ToLookup создает объект Lookup типа <К, Т> или, 
            //возможно, <К, Е> из входной последовательности типа T, 
            //где К — тип ключа, a T — тип хранимых значений. 
            //Либо же, если Lookup имеет тип <К, E>, то типом хранимых значений может быть Е, 
            //который отличается от типа элементов входной последовательности Т.
            ILookup<int, Actor> lookup = Actor.GetActors().ToLookup(k => k.birthYear);


            ILookup<string, Actor2> lookup2 = Actor2.GetActors()
        .ToLookup(k => k.birthYear, new MyStringifiedNumberComparer());

            IEnumerable<Actor2> actors2 = lookup2["0001964"];


            ILookup<int, string> lookup3 = Actor.GetActors()
        .ToLookup(k => k.birthYear,
                  a => string.Format("{0} {1}", a.firstName, a.lastName));

            IEnumerable<string> actors3 = lookup3[1964];


            ILookup<string, string> lookup4 = Actor2.GetActors()
        .ToLookup(k => k.birthYear,
                  a => string.Format("{0} {1}", a.firstName, a.lastName),
                  new MyStringifiedNumberComparer());
            #endregion

            #region Операция SequenceEqual определяет, эквивалентны ли две входные последовательности.
            bool eq = cars.SequenceEqual(cars.Take(cars.Count()));

            string[] strArr1 = { "0012", "130", "0000019", "4" };
            string[] strArr2 = { "12", "0130", "019", "0004" };

            bool eq2 = strArr1.SequenceEqual(strArr2,
          new MyStringifiedNumberComparer());

            #endregion

            #region Операции First, FirstOrDefault, Last и LastOrDefault
            //Операция First возвращает первый элемент последовательности 
            //или первый элемент последовательности, соответствующий предикату — 
            //в зависимости от использованного прототипа
            string auto20 = cars.First();
            string auto21 = cars.First(p => p.StartsWith("N"));

            //Операция FirstOrDefault подобна First во всем, кроме поведения, когда элемент не найден. 
            string auto22 = cars.Take(0).FirstOrDefault();

            //перация Last возвращает последний элемент последовательности или последний элемент, 
            //соответствующий предикату — в зависимости от используемого прототипа
            string auto23 = cars.Last();
            string auto24 = cars.Last(p => p.StartsWith("A"));

            //Операция LastOrDefault подобна Last во всем, за исключением поведения в случае, 
            //когда элемент не найден. 
            //Эта операция имеет два прототипа, анологичные операции Last.
            string auto25 = cars.Take(0).LastOrDefault();

            string auto26 = cars.LastOrDefault(p => p.StartsWith("A"));
            #endregion

            #region Операции Single, SingleOrDefault, ElementAt и ElementAtOrDefault
            //Операция Single возвращает единственный элемент последовательности 
            //или единственный элемент последовательности, соответствующий предикату —
            //в зависимости от используемого прототипа
            Employee emp = Employee.GetEmployeesArray().Where(ez => ez.id == 3).Single();

            Employee emp2 = Employee.GetEmployeesArray().Single(ez => ez.id == 3);
            //Операция SingleOrDefault подобна Single, но отличается поведением в случае, 
            //когда элемент не найден.
            Employee emp3 = Employee.GetEmployeesArray()
        .Where(ez => ez.id == 5).SingleOrDefault();

            //Операция ElementAt возвращает элемент из исходной последовательности по указанному индексу.
            string auto27 = cars.ElementAt(1);
            //Операция ElementAtOrDefault возвращает из исходной последовательности элемент, имеющий указанный индекс местоположения
            string auto28 = cars.ElementAtOrDefault(3);
            #endregion

            #region Операции Any, All и Contains
            //Операция Any возвращает true, если любой из элементов 
            //входной последовательности отвечает условию.
            bool anyCars = cars.Any();
            anyCars = cars.Any(s => s.StartsWith("X"));
            // Операция All возвращает true, если каждый элемент входной последовательности отвечает условию.
            bool all = cars.All(s => s.StartsWith("A"));
            all = cars.All(s => s.Length > 2);
            //Операция Contains возвращает true, если любой элемент входной последовательности 
            //соответствует указанному значению.
            bool contains = cars.Contains("Jaguar");

            string[] nums55 = { "1", "5", "10", "28" };
            bool contains2 = nums55.Contains("000028",
                            new MyStringifiedNumberComparer());
            //!!!!!!!!!!!!
            //Contains takes an object, <-
            //Any takes a predicate. <-
            //listOFInts.Contains(1);
            //listOfInts.Any(i => i == 1);
            //So if you want to check for a specific condition, use Any.If you want to check for the existence of an element, use Contains.
            #endregion


            #region Операции Count, LongCount и Sum
            //Операция Count возвращает количество элементов во входной последовательности. 
            int count = cars.Count();
            count = cars.Count(s => s.StartsWith("A"));
            //Операция LongCount возвращает количество элементов входной последовательности как значение типа long.
            long count2 = Enumerable.Range(0, int.MaxValue)
                .Concat(Enumerable.Range(0, int.MaxValue)).LongCount();
            count2 = Enumerable.Range(0, int.MaxValue)
              .Concat(Enumerable.Range(0, int.MaxValue))
             .LongCount(s => s % 2 == 0);
            //Операция Sum возвращает сумму числовых значений, содержащихся в элементах последовательности.
            IEnumerable<int> ints = Enumerable.Range(1, 100);

            IEnumerable<EmployeeOptionEntry> options =
        EmployeeOptionEntry.GetEmployeeOptionEntries();
            long optionsSum = options.Sum(o => o.optionsCount);

            #endregion

            #region Min и Max
            //Операция Min возвращает минимальное значение входной последовательности.
            int min = nums.Min();

            string auto30 = cars.Min();

            min = Actor.GetActors().Min(a => a.birthYear);

            string actor = Actor.GetActors().Min(a => a.lastName);

            //Операция Max возвращает максимальное значение из входной последовательности.
            int Max = nums.Max();

            string auto31 = cars.Max();

            Max = Actor.GetActors().Max(a => a.birthYear);

            string actor2 = Actor.GetActors().Max(a => a.lastName);
            #endregion

            #region Операции Average и Aggregate
            //Операция Average возвращает среднее арифметическое числовых значений элементов входной последовательности.
            double average = nums.Average();

            double optionAverage = options.Average(o => o.optionsCount);

            //Операция Aggregate выполняет указанную пользователем функцию на каждом элементе входной последовательности, 
            //передавая значение, возвращенное этой функцией для предыдущего элемента, и возвращая ее значение для последнего элемента.
            int agg = Enumerable
                         .Range(1, 10)
                         .Aggregate(0, (s, i) => s + i);
            #endregion


            //!!LINQ to XML
            XElement xEmployees =
               new XElement("Employees",
                   new XElement("Employee",
                       new XAttribute("type", "Programmer"),
                       new XElement("FirstName", "Alex"),
                       new XElement("LastName", "Erohin")),
                   new XElement("Employee",
                       new XAttribute("type", "Editor"),
                       new XElement("FirstName", "Elena"),
                       new XElement("LastName", "Volkova")));


            //https://professorweb.ru/my/LINQ/linq_xml/level5/5_2.php
            foreach (var item in auto7)
            {
            Console.WriteLine(item);
            }
                Console.ReadKey();
        }
    }
}
