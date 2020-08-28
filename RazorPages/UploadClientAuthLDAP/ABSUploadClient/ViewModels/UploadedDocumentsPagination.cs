using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABSUploadClient.ViewModels
{
    public class UploadedDocumentsPagination
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public UploadedDocumentsPagination(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
