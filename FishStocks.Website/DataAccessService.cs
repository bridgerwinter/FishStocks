using AntDesign;
using FishStocks.Library;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace FishStocks.Website
{
        public class DataAccessService
        {
            private IConfiguration config;
            public DataAccessService(IConfiguration configuration)
            {
                config = configuration;
            }
            private string ConnectionString
            {
                get
                {
                    string connectionString = @"Data Source=SUNBEAR\SQLEXPRESS;Initial Catalog = FishStocks; Integrated Security = True";
                    return connectionString;
                }
            }

            public async Task<List<FishTransaction>> GetTransactionsAsync()
            {
                List<FishTransaction> transactions = new List<FishTransaction>();
                FishTransaction s;
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("select * from FishTransaction", con);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    s = new FishTransaction();
                    s.Id = (int)row["ID"];
                    s.Name = (string)row["Name"];
                    s.DateEntered = (DateTime)row["DateEntered"];
                    s.Price = (float)row["Price"];
                    transactions.Add(s);
                }
                return await Task.FromResult(transactions);
            }

        }
    }
