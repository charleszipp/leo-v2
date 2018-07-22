using Phase.Interfaces;
using System;

namespace Phase.Tests.Commands
{
    public class LinkAccount : ICommand
    {
        public LinkAccount(Guid budgetId, Guid accountId, string accountNumer, string accountName)
        {
            BudgetId = budgetId;
            AccountId = accountId;
            AccountNumer = accountNumer;
            AccountName = accountName;
        }

        public Guid AccountId { get; }

        public string AccountName { get; }

        public string AccountNumer { get; }

        public Guid BudgetId { get; }
    }
}