
using System;
using System.Linq;
using System.Net.Mail;

namespace MedicalPortal.Models
{
    public class Patient : Person
    {
        public int PatientId => Id;
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }

        public Patient(
            int id,
            string fullName,
            DateTime birthDate,
            string phone,
            string email,
            string address)
            : base(id, fullName, birthDate)
        {
            (PhoneNumber, Email, Address) = ValidateProfile(phone, email, address);
        }

        public void UpdateProfile(
            string phone,
            string email,
            string address)
        {
            (PhoneNumber, Email, Address) = ValidateProfile(phone, email, address);
        }

        public int GetAge()
        {
            int age = DateTime.Today.Year - BirthDate.Year;
            return BirthDate.Date > DateTime.Today.AddYears(-age) ? age - 1 : age;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"ID: {PatientId}");
            Console.WriteLine($"ФИО: {FullName}");
            Console.WriteLine($"Возраст: {GetAge()}");
            Console.WriteLine($"Телефон: {PhoneNumber}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Адрес: {Address}");
        }

        private static (string Phone, string Email, string Address) ValidateProfile(
            string phone,
            string email,
            string address)
        {
            string normalizedPhone = phone?.Trim() ?? "";
            string normalizedEmail = email?.Trim() ?? "";
            string normalizedAddress = address?.Trim() ?? "";
            string digits = new string(normalizedPhone.Where(char.IsDigit).ToArray());

            if (digits.Length is < 7 or > 15 ||
                normalizedPhone.Any(character => !char.IsDigit(character) &&
                    character is not '+' and not '-' and not '(' and not ')' and not ' '))
                throw new ArgumentException("Укажите корректный номер телефона.");

            try
            {
                _ = new MailAddress(normalizedEmail);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Укажите корректный адрес электронной почты.");
            }

            if (string.IsNullOrWhiteSpace(normalizedAddress))
                throw new ArgumentException("Адрес обязателен.");

            return (normalizedPhone, normalizedEmail, normalizedAddress);
        }
    }
}
