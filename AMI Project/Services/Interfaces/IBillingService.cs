using AMI_Project.Models;
using System.Threading;

namespace AMI_Project.Services.Interfaces
{
    public interface IBillingService
    {
        public decimal? GenerateBill(string meterSerialNo);
    }
}
