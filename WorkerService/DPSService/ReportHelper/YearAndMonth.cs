namespace DPSService.ReportHelper
{
    public class YearAndMonth
    {
        public int year;
        public int month;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is YearAndMonth))
            {
                return false;
            }
            YearAndMonth yearAndMonth = obj as YearAndMonth;
            if (yearAndMonth.year == year && yearAndMonth.month == month)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return year.GetHashCode() + month.GetHashCode();
        }
    }
}
