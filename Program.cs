using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MedicalPortal.Core;
using MedicalPortal.Models;

namespace MedicalPortal
{
    internal static class Program
    {
        private static readonly MedicalPortalManager Portal = new();

        private static void Main()
        {
            bool running = true;
            while (running)
            {
                ShowMenu();
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1": AddPatient(); break;
                        case "2": AddDoctor(); break;
                        case "3": Portal.DisplayPatients(); break;
                        case "4": Portal.DisplayDoctors(); break;
                        case "5": CreateAppointment(); break;
                        case "6": Portal.CancelAppointment(ReadInt("Введите ID записи: ")); break;
                        case "7": Portal.ShowDoctorSchedule(ReadInt("Введите ID врача: ")); break;
                        case "8": AddMedicalTest(); break;
                        case "9": AddPrescription(); break;
                        case "10": MakePayment(); break;
                        case "11": Portal.SearchPatient(); break;
                        case "12": Portal.DisplayStatistics(); break;
                        case "13": running = false; break;
                        case "14": Portal.ConfirmAppointment(ReadInt("Введите ID записи: ")); break;
                        case "15": Portal.CompleteAppointment(ReadInt("Введите ID записи: ")); break;
                        case "16": Portal.RefundPayment(ReadInt("Введите ID платежа: ")); break;
                        case "17": AddMedicalService(); break;
                        default: Console.WriteLine("Неизвестный пункт меню."); break;
                    }
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine("Ошибка ввода: " + exception.Message);
                }
                catch (InvalidOperationException exception)
                {
                    Console.WriteLine("Операция не выполнена: " + exception.Message);
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.Write("Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    Console.WriteLine();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("========== MEDICAL PORTAL ==========");
            Console.WriteLine("1. Добавить пациента");
            Console.WriteLine("2. Добавить врача");
            Console.WriteLine("3. Показать всех пациентов");
            Console.WriteLine("4. Показать всех врачей");
            Console.WriteLine("5. Записать пациента на приём");
            Console.WriteLine("6. Отменить запись");
            Console.WriteLine("7. Показать расписание врача");
            Console.WriteLine("8. Добавить медицинский анализ");
            Console.WriteLine("9. Создать медицинское назначение");
            Console.WriteLine("10. Оплатить услугу");
            Console.WriteLine("11. Поиск пациента");
            Console.WriteLine("12. Статистика портала");
            Console.WriteLine("13. Выход");
            Console.WriteLine("14. Подтвердить запись");
            Console.WriteLine("15. Завершить приём");
            Console.WriteLine("16. Вернуть платёж");
            Console.WriteLine("17. Добавить медицинскую услугу");
        }

        private static void AddPatient()
        {
            int id = ReadInt("ID пациента: ");
            string fullName = ReadRequired("ФИО: ");
            DateTime birthDate = ReadDate("Дата рождения (например, 15.04.1990): ");
            string phone = ReadRequired("Телефон: ");
            string email = ReadRequired("Email: ");
            string address = ReadRequired("Адрес: ");
            Portal.AddPatient(new Patient(id, fullName, birthDate, phone, email, address));
        }

        private static void AddDoctor()
        {
            int id = ReadInt("ID врача: ");
            string fullName = ReadRequired("ФИО: ");
            DateTime birthDate = ReadDate("Дата рождения (например, 15.04.1980): ");
            string specialization = ReadRequired("Специализация: ");
            int experience = ReadInt("Стаж (лет): ");
            string cabinet = ReadRequired("Номер кабинета: ");
            decimal salary = ReadDecimal("Зарплата: ");
            Portal.AddDoctor(new Doctor(
                id, fullName, birthDate, specialization, experience, cabinet, salary));
        }

        private static void CreateAppointment()
        {
            Patient patient = FindPatientOrThrow(ReadInt("ID пациента: "));
            Doctor doctor = FindDoctorOrThrow(ReadInt("ID врача: "));
            DateTime date = ReadDate("Дата приёма: ");
            TimeSpan time = ReadTime("Время приёма (например, 14:30): ");
            int duration = ReadInt("Длительность приёма в минутах: ");

            Portal.CreateAppointment(
                NextId(Portal.Appointments, appointment => appointment.AppointmentId),
                patient,
                doctor,
                date,
                time,
                duration);
        }

        private static void AddMedicalTest()
        {
            Patient patient = FindPatientOrThrow(ReadInt("ID пациента: "));
            string name = ReadRequired("Название анализа: ");
            var test = new MedicalTest(
                NextId(Portal.MedicalTests, item => item.TestId), name, patient);
            Portal.AddMedicalTest(test);

            string result = ReadOptional("Результат (Enter, если пока ожидается): ");
            if (!string.IsNullOrWhiteSpace(result))
                test.AddResult(result);
        }

        private static void AddPrescription()
        {
            Patient patient = FindPatientOrThrow(ReadInt("ID пациента: "));
            Doctor doctor = FindDoctorOrThrow(ReadInt("ID врача: "));
            string medication = ReadRequired("Препарат: ");
            string dosage = ReadRequired("Дозировка: ");
            int duration = ReadInt("Продолжительность (дней): ");
            Portal.AddPrescription(new Prescription(
                NextId(Portal.Prescriptions, item => item.PrescriptionId),
                patient, doctor, medication, dosage, duration));
        }

        private static void MakePayment()
        {
            if (Portal.MedicalServices.Count == 0)
            {
                Console.WriteLine("Каталог услуг пуст. Сначала добавьте услугу (пункт 17).");
                return;
            }

            Patient patient = FindPatientOrThrow(ReadInt("ID пациента: "));
            Console.WriteLine("Доступные услуги:");
            foreach (MedicalService service in Portal.MedicalServices)
            {
                Console.Write($"{service.ServiceId}. ");
                service.DisplayService();
            }

            int serviceId = ReadInt("ID услуги: ");
            MedicalService selectedService = Portal.MedicalServices
                .FirstOrDefault(service => service.ServiceId == serviceId)
                ?? throw new InvalidOperationException("Медицинская услуга не найдена.");

            Portal.MakePayment(
                NextId(Portal.Payments, payment => payment.PaymentId),
                patient,
                selectedService.CalculatePrice());
        }

        private static void AddMedicalService()
        {
            string name = ReadRequired("Название услуги: ");
            string description = ReadRequired("Описание услуги: ");
            decimal price = ReadDecimal("Цена: ");
            int duration = ReadInt("Длительность (минут): ");
            Portal.AddMedicalService(new MedicalService(
                NextId(Portal.MedicalServices, service => service.ServiceId),
                name,
                description,
                price,
                duration));
        }

        private static Patient FindPatientOrThrow(int id) =>
            Portal.FindPatient(id) ?? throw new InvalidOperationException("Пациент не найден.");

        private static Doctor FindDoctorOrThrow(int id) =>
            Portal.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id)
            ?? throw new InvalidOperationException("Врач не найден.");

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                Console.WriteLine("Введите целое число.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value) ||
                    decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
                Console.WriteLine("Введите корректную сумму.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (DateTime.TryParseExact(input,
                        new[] { "dd.MM.yyyy", "yyyy-MM-dd", "MM/dd/yyyy" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value) ||
                    DateTime.TryParse(input, CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out value))
                    return value.Date;
                Console.WriteLine("Введите корректную дату.");
            }
        }

        private static TimeSpan ReadTime(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TimeSpan.TryParse(Console.ReadLine(), CultureInfo.CurrentCulture, out TimeSpan value) &&
                    value >= TimeSpan.Zero && value < TimeSpan.FromDays(1))
                    return value;
                Console.WriteLine("Введите время в формате ЧЧ:ММ.");
            }
        }

        private static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string value = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
                Console.WriteLine("Это поле обязательно.");
            }
        }

        private static string ReadOptional(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim() ?? "";
        }

        private static int NextId<T>(IEnumerable<T> items, Func<T, int> getId) =>
            items.Select(getId).DefaultIfEmpty(0).Max() + 1;
    }
}
