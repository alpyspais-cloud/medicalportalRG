
using System;

namespace MedicalPortal.Models
{
    public class Payment
    {
        public const string PendingStatus = "Ожидает оплаты";
        public const string PaidStatus = "Оплачено";
        public const string RefundedStatus = "Возвращено";

        public int PaymentId { get; }
        public Patient Patient { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string PaymentStatus { get; private set; } = PendingStatus;

        public Payment(int id, Patient patient, decimal amount)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(patient);
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Сумма должна быть больше нуля.");

            PaymentId = id;
            Patient = patient;
            Amount = amount;
            PaymentDate = DateTime.Now;
            PaymentStatus = "Ожидает оплаты";
        }

        public bool MakePayment()
        {
            if (PaymentStatus != PendingStatus)
            {
                Console.WriteLine(PaymentStatus == PaidStatus
                    ? "Платёж уже выполнен."
                    : "Возвращённый платёж нельзя оплатить повторно.");
                return false;
            }

            PaymentStatus = PaidStatus;
            PaymentDate = DateTime.Now;

            Console.WriteLine("Оплата успешно выполнена!");
            return true;
        }

        public bool RefundPayment()
        {
            if (PaymentStatus == PaidStatus)
            {
                PaymentStatus = RefundedStatus;
                Console.WriteLine("Деньги возвращены.");
                return true;
            }

            Console.WriteLine("Возврат возможен только для оплаченного платежа.");
            return false;
        }

        public void DisplayPayment()
        {
            Console.WriteLine("Платёж №" + PaymentId);
            Console.WriteLine("Пациент: " + Patient.FullName);
            Console.WriteLine("Сумма: " + Amount + " тг");
            Console.WriteLine("Дата: " + PaymentDate);
            Console.WriteLine("Статус: " + PaymentStatus);
        }
    }
}
