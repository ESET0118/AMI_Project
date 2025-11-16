using AMI_Project.Models;
using System.Threading;

namespace AMI_Project.Services.Interfaces
{
    public interface IBillingService
    {
        public BillDto? GenerateBill(string meterSerialNo);
    }
}
