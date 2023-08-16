using System;
using MySqlConnector;
using System.Collections.Generic;
using NorthLake.SmartSearch.Request;
using NorthLake.SmartSearch.Response;
using NorthLake.SmartSearch.Repository;
using System.Collections;
using System.Reflection.PortableExecutable;

namespace NorthLake.SmartSearch.Service
{
	public class SearchService
	{
		public SearchService()
		{

		}
        SearchData database = new SearchData();

        public List<SearchResponse> GetDataForSearch(SearchRequest request)
        {
           
            //open connection to database
           
            List<SearchResponse> list = new List<SearchResponse>();

            //Method requires a non null request and database
            if (request == null || database == null)
            {
                return list;
            }

             MySqlDataReader? reader = database.GetSearchQuery(request);

            if (reader == null)
            {
                return list;
            }


            try
            {
                SetDataForResponse(list, reader);
            }
            catch (ArgumentNullException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return list;
        }


        public List<SearchResponse> GetDataForSuggestions(string keyWord)
        { 
            SearchData database = new();

            
            List<SearchResponse> list = new();

            //Method requires a non null request and database
            if ( database == null || String.IsNullOrEmpty(keyWord))
            {
                return list;
            }


            MySqlDataReader? reader = database.GetSuggestionQuery(keyWord);

            try
            {
                SetDataForResponse(list, reader);
            }
            catch (ArgumentNullException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return list;

        }




        private void SetDataForResponse( List<SearchResponse> list,MySqlDataReader? reader)
        {
            if (list == null || reader == null)
            {
                throw new ArgumentNullException("Parameter cannot be null", nameof(reader));
            }
            //generate response and include in returned list
            try
            {
                while (reader.Read())
                {
                    SearchResponse response = new SearchResponse();
                    response.CompanyName = (string)reader["Company_name"];
                    response.FilingDate = (reader["DATE_FORMAT(Filing_Date, '%M %d %Y')"] == DBNull.Value) ? "N/A" : (string)reader["DATE_FORMAT(Filing_Date, '%M %d %Y')"];
                    response.FormType =  (reader["Form_Type"] == DBNull.Value) ? "N/A" : (string)reader["Form_Type"];
                    response.SEC =  (reader["SEC_number"]  == DBNull.Value) ? "N/A" : (string)reader["SEC_number"];
                    list.Add(response);
                }

            }
            finally
            {
                //close reader and connection
                reader.Close();
                database.Conn.Close();
            }
        }

    }
}

