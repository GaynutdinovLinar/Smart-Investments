using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Investments.Services.LocationDataBaseService.UserStocks
{
    public class Enrollment
    {
        public Enrollment(DateTime dateOfConclusion, DateTime settlementDate, string operationType, double amount,
            string currency, string debitingFrom, string creditingTo, string operationContent, string status)
        {
            DateOfConclusion = dateOfConclusion;
            SettlementDate = settlementDate;
            OperationType = operationType;
            Currency = currency;
            DebitingFrom = debitingFrom;
            CreditingTo = creditingTo;
            OperationContent = operationContent;
            Status = status;
            Amount = amount;
        }

        public Enrollment(DateTime dateOfConclusion, DateTime settlementDate, string operationType, double amount,
            string currency, string debitingFrom, string creditingTo, string secid, int count, string status)
        {
            DateOfConclusion = dateOfConclusion;
            SettlementDate = settlementDate;
            OperationType = operationType;
            Currency = currency;
            DebitingFrom = debitingFrom;
            CreditingTo = creditingTo;
            Shortname = secid;
            Count = count;
            Status = status;
            Amount = amount;
        }


        public DateTime DateOfConclusion { get; private set; }

        public DateTime SettlementDate { get; private set; }

        public string OperationType { get; private set; }

        public double Amount { get; private set; }

        public string Currency { get; private set; }

        public string DebitingFrom { get; private set; }

        public string CreditingTo { get; private set; }

        public string OperationContent { get; private set; }

        public string Shortname { get; private set; }

        public int Count { get; private set; }

        public string Status { get; private set; }


    }
}
