using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;
using MySqlConnector;
using NorthLake.SmartSearch.Request;
using NorthLake.SmartSearch.Response;

namespace NorthLake.SmartSearch.Repository
{
    public class SearchData
    {
        public MySqlConnection Conn
        {
            get;
            set;
        }

        
        public SearchData()
        {
            Conn = new MySqlConnection("Server = database-1.cvt3rfrvkrjd.us-east-1.rds.amazonaws.com; UserID = admin;Password = 12345678;Database = web_scraper");
        }


       public MySqlDataReader? GetSearchQuery(SearchRequest request)
        {
            Conn.Open();

            // get request variables
            int CIK = string.IsNullOrEmpty(request.CompanyCIK) ? 0 : int.Parse(request.CompanyCIK);
            var CompanyName = request.CompanyName.ToLower();
            string FormType = request.FormType.ToLower();

            if (CIK == 0 && CompanyName == "" && FormType == "")
            {
                return null;
            }


            //generate where condition
            var whereCondition = GetWhereCondition(request);
            if (string.IsNullOrEmpty(whereCondition))
            {
                return null;
            }

            string query = $"SELECT Company_name, DATE_FORMAT(Filing_Date, '%M %d %Y'), Form_Type, SEC_number " +
                $"FROM Company LEFT JOIN CompanyFilings USING(CIK) WHERE {whereCondition}";

            MySqlCommand command = new(query, Conn);
            MySqlDataReader reader = command.ExecuteReader();

            

            return reader;
            ;
        }

        public MySqlDataReader? GetSuggestionQuery(string keyWord)
        {
            Conn.Open();

            // search for company names that might have substring of the user's input
            string query = $"SELECT Company_name, DATE_FORMAT(Filing_Date, '%M %d %Y'), Form_Type, SEC_number " +
                $"FROM Company LEFT JOIN CompanyFilings USING(CIK) WHERE Company_name LIKE '%{keyWord}%'";

            MySqlCommand command = new MySqlCommand(query, Conn);

            if (command == null)
            {
                return null; 
            }

            MySqlDataReader reader = command.ExecuteReader();


            return reader;
        }


        private string GetWhereCondition(SearchRequest request)
        {
            //StringBuilder queryBuilder = new StringBuilder();


            //if (!string.IsNullOrEmpty(request.CompanyCIK))
            //{
            //    queryBuilder.Append($"CIK = {request.CompanyCIK}");
            //    // there will be conditions coming afterwards
            //    if (!string.IsNullOrEmpty(request.CompanyName) || !string.IsNullOrEmpty(request.FormType))
            //    {
            //        queryBuilder.Append(" AND ");
            //    }
            //}

            //if (!string.IsNullOrEmpty(request.CompanyName))
            //{
            //    queryBuilder.Append($"Company_name LIKE '%{request.CompanyName}%'");
            //    // there will be conditions coming afterwards
            //    if (!string.IsNullOrEmpty(request.FormType))
            //    {
            //        queryBuilder.Append(" AND ");
            //    }
            //}

            //if (!string.IsNullOrEmpty(request.FormType))
            //{
            //    queryBuilder.Append($"Form_Type LIKE '%{request.FormType}%'");
            //}

            //return queryBuilder.ToString();


            StringBuilder queryBuilder = new();
            List<string> andConditions = new();
            
            if (!string.IsNullOrEmpty(request.CompanyCIK))
            {
                andConditions.Add($"CIK = {request.CompanyCIK}");
               
            }

            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                andConditions.Add($"Company_name LIKE '%{request.CompanyName.Trim()}%'");
                
            }

            if (!string.IsNullOrEmpty(request.FormType))
            {
                andConditions.Add($"Form_Type LIKE '{request.FormType.Trim()}'");
            }

            queryBuilder.Append(String.Join(" AND ", andConditions));
           
            
            return queryBuilder.ToString();
        }

    }
}

