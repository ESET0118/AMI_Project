using AMI_Project.Data.Models;
using System.Threading;

namespace AMI_Project.Services.Interfaces
{
    public interface IBillingService
    {
        public BillDto? GenerateBill(string meterSerialNo);
        public Bill SaveBill(string meterSerialNo, decimal amount, long consumerId);
        public List<Bill> GetAllBills();
        public List<Bill> GetBillsByMeter(string meterSerialNo);
        public List<Bill> GetBillsByConsumer(long consumerId);



    }
}