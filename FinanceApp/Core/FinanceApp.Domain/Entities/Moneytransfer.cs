using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Domain.Entities
{
	public class Moneytransfer
	{
		public int Id { get; set; }
		public int SenderAccountId { get; set; }
		public int ReceiverAccountId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public DateTime TransferDate { get; set; }
		public TransferStatus Status { get; set; }
		public string ReferenceNumber { get; set; }

		public virtual Account SenderAccount { get; set; }
		public virtual Account ReceiverAccount { get; set; }

	}
	public enum TransferStatus
	{
		Beklemede = 0,
		Tamamlandi = 1,
		Basarisiz = 2,
		Iptal = 3
	}
}
