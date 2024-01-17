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


            // Database loading

            foreach (var customer in customers)
            {
                // Add the customer to the context
                context.Customers.Add(customer);

                foreach (var account in customer.Accounts)
                {
                    // Set the foreign key for the account
                    account.CustomerID = customer.CustomerID;

                    // Check if the account already exists to avoid primary key violation
                    if (!context.Accounts.Any(a => a.AccountNumber == account.AccountNumber))
                    {
                        context.Accounts.Add(account);
                    }

                    var balanceTotal = 0M; // Initialize balance total

                    foreach (var transaction in account.Transactions)
                    {
                        // Set required properties for the transaction
                        transaction.AccountNumber = account.AccountNumber;
                        transaction.TransactionType = 'D'; // Assume all transactions are deposits

                        // Update the running total balance
                        balanceTotal += transaction.Amount;

                        // Check if the transaction already exists to avoid primary key violation
                        if (!context.Transactions.Any(t => t.TransactionID == transaction.TransactionID))
                        {
                            context.Transactions.Add(transaction);
                        }
                    }

                    // Update the balance for the account
                    account.Balance = balanceTotal;
                }

                // Set foreign key for the customer's login
                customer.Login.CustomerID = customer.CustomerID;

                // Check if the login already exists to avoid primary key violation
                if (!context.Logins.Any(l => l.LoginID == customer.Login.LoginID))
                {
                    context.Logins.Add(customer.Login);
                }
            }

            // Save all changes at once after processing all entities
            context.SaveChanges();




        }
    }
}
