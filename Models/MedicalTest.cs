
using System;

namespace MedicalPortal.Models
{
    public class MedicalTest
    {
        public int TestId { get; }
        public string TestName { get; }
        public DateTime TestDate { get; }
        public string Result { get; private set; } = "";
        public Patient Patient { get; private set; }

        public MedicalTest(int id, string name, Patient patient)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название анализа обязательно.", nameof(name));
            ArgumentNullException.ThrowIfNull(patient);

            TestId = id;
            TestName = name.Trim();
            Patient = patient;
            TestDate = DateTime.Now;
            Result = "Ожидается";
        }

        public void AddResult(string result)
        {
            Result = ValidateResult(result);
            Console.WriteLine("Результат анализа добавлен.");
        }

        public void UpdateResult(string result)
        {
            Result = ValidateResult(result);
            Console.WriteLine("Результат анализа обновлён.");
        }

        public void DisplayResult()
        {
            Console.WriteLine("Анализ №" + TestId);
            Console.WriteLine("Название: " + TestName);
            Console.WriteLine("Пациент: " + Patient.FullName);
            Console.WriteLine("Дата: " + TestDate);
            Console.WriteLine("Результат: " + Result);
        }

        private static string ValidateResult(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new ArgumentException("Результат анализа обязателен.", nameof(result));
            return result.Trim();
        }
    }
}
