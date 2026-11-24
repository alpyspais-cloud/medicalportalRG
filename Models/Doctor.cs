
using System;

namespace MedicalPortal.Models
{
    public class Doctor : Person, IPayable
    {
        public int DoctorId => Id;
        public string Specialization { get; private set; }
        public int Experience { get; private set; }
        public string CabinetNumber { get; private set; }
        public decimal Salary { get; private set; }

        public Doctor(
            int id,
            string fullName,
            DateTime birthDate,
            string specialization,
            int experience,
            string cabinetNumber,
            decimal salary)
            : base(id, fullName, birthDate)
        {
            Specialization = ValidateText(specialization, nameof(specialization));
            if (experience < 0)
                throw new ArgumentOutOfRangeException(nameof(experience), "Стаж не может быть отрицательным.");
            if (salary < 0)
                throw new ArgumentOutOfRangeException(nameof(salary), "Зарплата не может быть отрицательной.");

            Experience = experience;
            CabinetNumber = ValidateText(cabinetNumber, nameof(cabinetNumber));
            Salary = salary;
        }

        public void UpdateSpecialization(string specialization)
        {
            Specialization = ValidateText(specialization, nameof(specialization));
        }

        public decimal CalculateSalary()
        {
            return Salary;
        }

        public decimal CalculatePayment()
        {
            return Salary;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"ID: {DoctorId}");
            Console.WriteLine($"Врач: {FullName}");
            Console.WriteLine($"Специализация: {Specialization}");
            Console.WriteLine($"Стаж: {Experience} лет");
            Console.WriteLine($"Кабинет: {CabinetNumber}");
            Console.WriteLine($"Зарплата: {Salary}");
        }

        private static string ValidateText(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Значение обязательно.", parameterName);

            return value.Trim();
        }
    }
}
