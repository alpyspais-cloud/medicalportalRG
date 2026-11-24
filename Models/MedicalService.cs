
using System;

namespace MedicalPortal.Models
{
    public class MedicalService
    {
        public int ServiceId { get; }
        public string ServiceName { get; }
        public string Description { get; }
        public decimal Price { get; private set; }
        public int Duration { get; }

        public MedicalService(
            int id,
            string name,
            string description,
            decimal price,
            int duration)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название услуги обязательно.", nameof(name));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной.");
            if (duration <= 0)
                throw new ArgumentOutOfRangeException(nameof(duration), "Длительность должна быть больше нуля.");

            ServiceId = id;
            ServiceName = name.Trim();
            Description = description?.Trim() ?? "";
            Price = price;
            Duration = duration;
        }

        public decimal CalculatePrice()
        {
            return Price;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice >= 0)
            {
                Price = newPrice;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Цена не может быть отрицательной.");
            }
        }

        public void DisplayService()
        {
            Console.WriteLine("Услуга: " + ServiceName);
            Console.WriteLine("Описание: " + Description);
            Console.WriteLine("Цена: " + Price + " тг");
            Console.WriteLine("Длительность: " + Duration + " минут");
        }
    }
}
