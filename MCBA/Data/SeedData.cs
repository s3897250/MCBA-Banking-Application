using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MCBA.Data;
using MCBA.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleHashing.Net;

namespace MCBA.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MCBAContext>();

            if (context.Customers.Any())
                return; // DB has already been seeded

            const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

            using var client = new HttpClient();
            var json = client.GetStringAsync(Url).Result;

            var jsonString = @"
            {
                ""Customers"": [
                    {
                        ""CustomerID"": 2100,
                        ""Name"": ""Matthew Bolger"",
                        ""TFN"": ""123 456 789"",
                        ""Mobile"": ""0412 345 678"",
                        ""State"": ""VIC"",
                        ""Address"": ""123 Fake Street"",
                        ""City"": ""Melbourne"",
                        ""PostCode"": ""3000"",
                        ""Accounts"": [
                            {
                                ""AccountNumber"": 4100,
                                ""AccountType"": ""S"",
                                ""CustomerID"": 2100,
                                ""Transactions"": [
                                    {
                                        ""Amount"": 100.00,
                                        ""Comment"": ""Opening balance"",
                                        ""TransactionTimeUtc"": ""02/01/2024 08:00:00 PM""
                                    }
                                ],
                                ""BillPays"": [
                                    {
                                        ""AccountNumber"": 4100,
                                        ""PayeeID"": 1,
                                        ""Amount"": 50.00,
                                        ""ScheduleTimeUtc"": ""2024-03-01T10:00:00Z"",
                                        ""Period"": ""M""
                                    }
                                ]
                            }
                        ],
                        ""Login"": {
                            ""LoginID"": ""12345678"",
                            ""PasswordHash"": ""Rfc2898DeriveBytes$50000$MrW2CQoJvjPMlynGLkGFrg==$x8iV0TiDbEXndl0Fg8V3Rw91j5f5nztWK1zu7eQa0EE=""
                        }
                    }
                ],
                ""Payees"": [
                    {
                        ""Name"": ""Utility Company"",
                        ""Address"": ""789 Utility Rd"",
                        ""City"": ""Melbourne"",
                        ""State"": ""VIC"",
                        ""Postcode"": ""3006"",
                        ""Phone"": ""0399991234""
                    },
                    {
                        ""Name"": ""Internet Service Provider"",
                        ""Address"": ""123 Web Street"",
                        ""City"": ""Sydney"",
                        ""State"": ""NSW"",
                        ""Postcode"": ""2001"",
                        ""Phone"": ""0298765432""
                    },
                    {
                        ""Name"": ""Health Insurance"",
                        ""Address"": ""456 Healthy Lane"",
                        ""City"": ""Brisbane"",
                        ""State"": ""QLD"",
                        ""Postcode"": ""4000"",
                        ""Phone"": ""0733334444""
                    }
                ]
            }";


            var data = JsonConvert.DeserializeObject<SeedDataModel>(jsonString, new JsonSerializerSettings  // json or jsonString
            {
                DateFormatString = "dd/MM/yyyy hh:mm:ss tt",
            });

            if (data == null)
                return;

            // Process customers
            foreach (var customer in data.Customers)
            {
                // Add the customer to the context
                context.Customers.Add(customer);

                // Process accounts and transactions
                foreach (var account in customer.Accounts)
                {
                    account.CustomerID = customer.CustomerID;

                    if (!context.Accounts.Any(a => a.AccountNumber == account.AccountNumber))
                        context.Accounts.Add(account);

                    var balanceTotal = 0M;

                    foreach (var transaction in account.Transactions)
                    {
                        transaction.AccountNumber = account.AccountNumber;
                        transaction.TransactionType = 'D';

                        balanceTotal += transaction.Amount;

                        if (!context.Transactions.Any(t => t.TransactionID == transaction.TransactionID))
                            context.Transactions.Add(transaction);
                    }

                    account.Balance = balanceTotal;

                    // Process bill payments
                    foreach (var billPay in account.BillPays)
                    {
                        billPay.AccountNumber = account.AccountNumber;

                        if (!context.BillPays.Any(b => b.BillPayID == billPay.BillPayID))
                            context.BillPays.Add(billPay);
                    }
                }

                customer.Login.CustomerID = customer.CustomerID;

                if (!context.Logins.Any(l => l.LoginID == customer.Login.LoginID))
                    context.Logins.Add(customer.Login);
            }

            // Process payees
            foreach (var payee in data.Payees)
            {
                if (!context.Payees.Any(p => p.PayeeID == payee.PayeeID))
                    context.Payees.Add(payee);
            }

            context.SaveChanges();
        }
        public static void SeedTestData(MCBAContext context)
        {
            if (context.Customers.Any(c => c.CustomerID == 2100))
                return;

            var customer = new Customer
            {
                CustomerID = 2100,
                Name = "Test Customer",
                Address = "111 Test Street",
                City = "Testville",
                PostCode = "1234",
                State = "TS",
                TFN = "123 456 789",
                Mobile = "0412 345 678",
                Login = new Login
                {
                    LoginID = "12345678",
                    PasswordHash = new SimpleHash().Compute("abc111"),
                    CustomerID = 2100
                },
                Accounts =
                [
                    new Account
                    {
                        AccountNumber = 4100,
                        AccountType = "S",
                        Balance = 1000,
                        CustomerID = 2100,

                        BillPays =
                        [
                            new BillPay
                            {
                                AccountNumber = 4100,
                                PayeeID = 1,
                                Amount = 60,
                                ScheduleTimeUtc = DateTime.UtcNow.AddDays(30),
                                Period = 'M',
                            }
                        ]
                    }
                ]
            };
            context.Payees.Add(new Payee { PayeeID = 1, Name = "Testing Company", Address = "789 Testing Rd", City = "Melbourne", State = "VIC", Postcode = "3006", Phone = "(03) 9999 1234" });


            context.Customers.Add(customer);

            context.SaveChanges();

        }
    }

    // Helper class to match the structure of your JSON
    public class SeedDataModel
    {
        public List<Customer> Customers { get; set; }
        public List<Payee> Payees { get; set; }
    }
}
