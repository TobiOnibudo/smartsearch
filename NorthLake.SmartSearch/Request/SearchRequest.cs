using System;
namespace NorthLake.SmartSearch.Request
{
    public class SearchRequest
    {

        public string? CompanyCIK
        {
            get;
            set;
        }
        public string CompanyName
        {
            get;
            set;
        }
        public string FormType
        {
            get;
            set;
        }
    }
}

