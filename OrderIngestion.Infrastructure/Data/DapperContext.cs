using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace OrderIngestion.Infrastructure.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;
        private readonly int _retryCount = 10;
        private readonly int _retryDelaySeconds = 5;

        public DapperContext(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        public IDbConnection CreateConnection()
        {
            var policy = Policy
                .Handle<SqlException>()
                .WaitAndRetry(
                    retryCount: _retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(_retryDelaySeconds),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"SQL Server not ready. Retry {retryCount}/{_retryCount} in {timeSpan.TotalSeconds}s...");
                    });

            IDbConnection connection = null;

            policy.Execute(() =>
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();
            });

            return connection;


            //int attempt = 0;

            //while (true)
            //{
            //    try
            //    {
            //        var connection = new SqlConnection(_connectionString);
            //        connection.Open(); // Try to open connection
            //        return connection;
            //    }
            //    catch (SqlException)
            //    {
            //        attempt++;
            //        if (attempt >= _maxRetries)
            //            throw; // Give up after max retries

            //        Console.WriteLine($"SQL Server not ready. Retry {attempt}/{_maxRetries} in {_delaySeconds} seconds...");
            //        Thread.Sleep(_delaySeconds * 1000);
            //    }
            //}
        }
    }
}
