﻿using Cryptocurrencies_info.Models.Cryptocurrencies;
using Cryptocurrencies_info.Models.DataBase;
using Microsoft.Data.SqlClient;

namespace Cryptocurrencies_info.Services.DataBase
{
    public class MsSql : IConnection
    {
        // Hardcodes
        private const string tableName = "CoinMarket";
        private const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=cryptocurrencies;Trusted_Connection=True;";

        // Add markets to sql
        public async Task AddMarkets(Market[] markets)
        {
            string marketStr = string.Join(",", markets
                .Select(market => $"('{market.Name}', '{market.Base}', '{market.Target}', '{market.Trust}', '{market.Link}', '{market.Logo}')"));
            string query = @$"INSERT INTO {tableName} (Name, Base, Target, Trust, Link, Logo)
        SELECT Name, Base, Target, Trust, Link, Logo
        FROM (
            VALUES {marketStr}
        ) AS Market(Name, Base, Target, Trust, Link, Logo)
        WHERE NOT EXISTS (
            SELECT 1 FROM {tableName} WHERE Name = Market.Name AND Base = Market.Base AND Target = Market.Target
        );";
            await MakeQuery(query);
        }

        // Delete all data in sql
        public void RefreshTable()
        {
            _ = MakeQuery($"TRUNCATE TABLE {tableName}");
        }

        // Making a query
        private static async Task MakeQuery(string sql)
        {
            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            SqlCommand command = new(sql, connection);
            _ = await command.ExecuteNonQueryAsync();
        }

        // Read and return data from sql
        public Market[] GetMarkets(IEnumerable<MarketBase> markets)
        {
            // Initialize variables, which will be used for making a query
            string names = String.Join(" OR ", markets
                .GroupBy(market => market.Name)
                .Select(market => $"Name = '{market.Key}'"));
            string identifires = String.Join(" OR ", markets
                .GroupBy(market => $"Base = '{market.Base}' AND Target = '{market.Target}'")
                .Select(market => market.Key));

            // Final str command
            string commandStr = $"SELECT * FROM {tableName} WHERE ({names}) AND {identifires}";

            // Run a connection
            using SqlConnection connection = new(connectionString);
            // Working with SQL
            connection.Open();
            SqlCommand command = new(commandStr, connection);

            // Read and initialize data
            using SqlDataReader reader = command.ExecuteReader();
            return IConnection.ParseMarkets(reader);
        }
    }
}