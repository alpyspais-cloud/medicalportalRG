
using System;

namespace MedicalPortal.Models
{
    public abstract class Person
    {
        public int Id { get; protected set; }
        public string FullName { get; protected set; } = "";
        public DateTime BirthDate { get; protected set; }

        protected Person(int id, string fullName, DateTime birthDate)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть положительным.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("ФИО обязательно.", nameof(fullName));
            if (birthDate.Date > DateTime.Today)
                throw new ArgumentException("Дата рождения не может быть в будущем.", nameof(birthDate));

            Id = id;
            FullName = fullName.Trim();
            BirthDate = birthDate;
        }

        public abstract void DisplayInfo();
    }
}
