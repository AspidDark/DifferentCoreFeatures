namespace ElasticLogBuilder
{
    /// <summary>
    /// ��������, ��������� ���������
    /// </summary>
    public class ElasticLogOperator
    {
        /// <summary>
        /// ��� ���������
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// ����� ���������
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// ����� ���������. 0 - ����������
        /// </summary>
        public long SubdivisionId { get; set; }
    }
}
