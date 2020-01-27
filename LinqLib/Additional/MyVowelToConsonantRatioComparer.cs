using System.Collections.Generic;

namespace LinqLib.Additional
{
    public class MyVowelToConsonantRatioComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            int vCount1 = 0;
            int cCount1 = 0;
            int vCount2 = 0;
            int cCount2 = 0;

            GetVowelConsonantCount(s1, ref vCount1, ref cCount1);
            GetVowelConsonantCount(s2, ref vCount2, ref cCount2);

            double dRatio1 = (double)vCount1 / (double)cCount1;
            double dRatio2 = (double)vCount2 / (double)cCount2;

            if (dRatio1 < dRatio2)
                return (-1);
            else if (dRatio1 > dRatio2)
                return (1);
            else
                return (0);
        }

        //  Это общедоступный метод, так что код, использующий 
        //  данный компаратор, может получить нужные значения
        public void GetVowelConsonantCount(string s,
                                           ref int vowelCount,
                                           ref int consonantCount)
        {
            // Гласные

            string vowels = "AEIOUYАЕЁИОУЫЭЮЯ";

            // Инициализация счетчиков
            vowelCount = 0;
            consonantCount = 0;

            // Преобразуем в верхний регистр, чтобы избежать зависимости от регистра
            string sUpper = s.ToUpper();

            foreach (char ch in sUpper)
            {
                if (vowels.IndexOf(ch) < 0)
                    consonantCount++;
                else
                    vowelCount++;
            }

            return;
        }
    }
}
