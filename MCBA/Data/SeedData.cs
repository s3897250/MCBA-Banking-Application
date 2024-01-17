using System;
using System.Net.Http;
using Newtonsoft.Json;
using MCBA.Models;
using MCBA.Data;
using System.Collections.Generic;
using System.Linq;

namespace MCBA.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MCBAContext>();

            // Check if records exist in the database
            if (context.Customers.Any())
                return; // DB has already been seeded

            const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

            using var client = new HttpClient();
            var json = client.GetStringAsync(Url).Result;

            var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy hh:mm:ss tt",
            });

            // Insert data into the database
            foreach (var customer in customers)
            {
                // Add customer to the context
                context.Customers.Add(customer);

                foreach (var account in customer.Accounts)
                {
                    // Set CustomerID for the account and add it to the context
                    account.CustomerID = customer.CustomerID;
                    context.Accounts.Add(account);

                    var balanceTotal = 0M;

                    foreach (var transaction in account.Transactions)
                    {
                        // Set AccountNumber and TransactionType for the transaction
                        transaction.AccountNumber = account.AccountNumber;
                        transaction.TransactionType = TransactionType.Deposit; // Assuming all transactions are deposits

                        // Add transaction amount to balance total and add transaction to the context
                        balanceTotal += transaction.Amount;
                        context.Transactions.Add(transaction);
                    }

                    // After transactions are inserted, update the account's balance
                    account.Balance = balanceTotal;
                }

                // Check if the customer has a login before adding it to the database
                if (customer.Login != null)
                {
                    // Set CustomerID for the login and add it to the context
                    customer.Login.CustomerID = customer.CustomerID;
                    context.Logins.Add(customer.Login);
                }
            }

            // Save all changes at the end
            context.SaveChanges();
        }
    }
}
