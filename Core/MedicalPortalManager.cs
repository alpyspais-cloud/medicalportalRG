using System;
using System.Collections.Generic;
using System.Linq;
using MedicalPortal.Models;

namespace MedicalPortal.Core
{
    public class MedicalPortalManager
    {
        public List<Patient> Patients { get; } = new();
        public List<Doctor> Doctors { get; } = new();
        public List<Appointment> Appointments { get; } = new();
        public List<MedicalRecord> MedicalRecords { get; } = new();
        public List<MedicalTest> MedicalTests { get; } = new();
        public List<MedicalTest> Tests => MedicalTests;
        public List<MedicalService> MedicalServices { get; } = new();
        public List<Payment> Payments { get; } = new();
        public List<Prescription> Prescriptions { get; } = new();
        public List<Department> Departments { get; } = new();

        public void AddPatient(Patient patient)
        {
            ArgumentNullException.ThrowIfNull(patient);
            EnsureUniqueId(Patients, patient.PatientId, item => item.PatientId, "пациента");
            Patients.Add(patient);
            Console.WriteLine("Пациент добавлен!");
        }

        public void AddDoctor(Doctor doctor)
        {
            ArgumentNullException.ThrowIfNull(doctor);
            EnsureUniqueId(Doctors, doctor.DoctorId, item => item.DoctorId, "врача");
            Doctors.Add(doctor);
            Console.WriteLine("Врач добавлен!");
        }

        public void AddMedicalService(MedicalService service)
        {
            ArgumentNullException.ThrowIfNull(service);
            EnsureUniqueId(MedicalServices, service.ServiceId, item => item.ServiceId, "услуги");
            MedicalServices.Add(service);
        }

        public void AddDepartment(Department department)
        {
            ArgumentNullException.ThrowIfNull(department);
            EnsureUniqueId(Departments, department.DepartmentId, item => item.DepartmentId, "отделения");
            Departments.Add(department);
        }

        public void AddMedicalRecord(MedicalRecord record)
        {
            ArgumentNullException.ThrowIfNull(record);
            EnsureUniqueId(MedicalRecords, record.RecordId, item => item.RecordId, "медицинской карты");
            MedicalRecords.Add(record);
        }

        public void DisplayPatients()
        {
            if (Patients.Count == 0)
            {
                Console.WriteLine("Пациенты не найдены.");
                return;
            }

            foreach (Patient patient in Patients)
            {
                patient.DisplayInfo();
                Console.WriteLine("----------------");
            }
        }

        public void DisplayDoctors()
        {
            if (Doctors.Count == 0)
            {
                Console.WriteLine("Врачи не найдены.");
                return;
            }

            foreach (Doctor doctor in Doctors)
            {
                doctor.DisplayInfo();
                Console.WriteLine("----------------");
            }
        }

        public Patient? SearchPatientById(int id)
        {
            return DisplaySearchResult(Patients.FirstOrDefault(patient => patient.PatientId == id));
        }

        public Patient? SearchPatientByName(string name)
        {
            Patient? patient = string.IsNullOrWhiteSpace(name)
                ? null
                : Patients.FirstOrDefault(candidate =>
                    candidate.FullName.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));
            return DisplaySearchResult(patient);
        }

        public Patient? SearchPatientByPhone(string phone)
        {
            Patient? patient = string.IsNullOrWhiteSpace(phone)
                ? null
                : Patients.FirstOrDefault(candidate =>
                    string.Equals(candidate.PhoneNumber, phone.Trim(), StringComparison.OrdinalIgnoreCase));
            return DisplaySearchResult(patient);
        }

        public Patient? FindPatient(int id) =>
            Patients.FirstOrDefault(patient => patient.PatientId == id);

        public Patient? FindPatientByName(string name) =>
            string.IsNullOrWhiteSpace(name)
                ? null
                : Patients.FirstOrDefault(patient =>
                    patient.FullName.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

        public Patient? FindPatientByPhone(string phone) =>
            string.IsNullOrWhiteSpace(phone)
                ? null
                : Patients.FirstOrDefault(patient =>
                    string.Equals(patient.PhoneNumber, phone.Trim(), StringComparison.OrdinalIgnoreCase));

        public void SearchPatient()
        {
            Console.WriteLine("1. Поиск по ID");
            Console.WriteLine("2. Поиск по ФИО");
            Console.WriteLine("3. Поиск по телефону");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Введите ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                        SearchPatientById(id);
                    else
                        Console.WriteLine("Некорректный ID.");
                    break;
                case "2":
                    Console.Write("Введите ФИО: ");
                    SearchPatientByName(Console.ReadLine() ?? "");
                    break;
                case "3":
                    Console.Write("Введите телефон: ");
                    SearchPatientByPhone(Console.ReadLine() ?? "");
                    break;
                default:
                    Console.WriteLine("Неизвестный способ поиска.");
                    break;
            }
        }

        public void CreateAppointment(
            int id,
            Patient patient,
            Doctor doctor,
            DateTime date,
            TimeSpan time,
            int durationMinutes = 30)
        {
            ArgumentNullException.ThrowIfNull(patient);
            ArgumentNullException.ThrowIfNull(doctor);
            if (!Patients.Contains(patient) || !Doctors.Contains(doctor))
                throw new InvalidOperationException("Пациент и врач должны быть зарегистрированы в портале.");
            EnsureUniqueId(Appointments, id, appointment => appointment.AppointmentId, "записи");

            Appointment appointment = new(id, patient, doctor, date, time, durationMinutes);
            if (!Appointment.IsDoctorAvailable(
                    doctor, date, time, Appointments, durationMinutes))
                return;

            Appointments.Add(appointment);
            appointment.CreateAppointment();
        }

        public void CancelAppointment(int id)
        {
            Appointment? appointment = Appointments.FirstOrDefault(item => item.AppointmentId == id);
            if (appointment is null)
            {
                Console.WriteLine("Запись не найдена!");
                return;
            }

            if (appointment.Status == AppointmentStatus.Completed)
            {
                Console.WriteLine("Завершённую запись нельзя отменить.");
                return;
            }

            appointment.CancelAppointment();
        }

        public void ConfirmAppointment(int id)
        {
            Appointment? appointment = Appointments.FirstOrDefault(item => item.AppointmentId == id);
            if (appointment is null)
            {
                Console.WriteLine("Запись не найдена!");
                return;
            }

            try
            {
                appointment.ConfirmAppointment();
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void CompleteAppointment(int id)
        {
            Appointment? appointment = Appointments.FirstOrDefault(item => item.AppointmentId == id);
            if (appointment is null)
            {
                Console.WriteLine("Запись не найдена!");
                return;
            }

            try
            {
                appointment.CompleteAppointment();
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void ShowDoctorSchedule(int doctorId)
        {
            if (!Doctors.Any(doctor => doctor.DoctorId == doctorId))
            {
                Console.WriteLine("Врач не найден.");
                return;
            }

            List<Appointment> schedule = Appointments
                .Where(appointment => appointment.Doctor.DoctorId == doctorId)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ThenBy(appointment => appointment.AppointmentTime)
                .ToList();
            if (schedule.Count == 0)
            {
                Console.WriteLine("У врача пока нет записей.");
                return;
            }

            foreach (Appointment appointment in schedule)
            {
                appointment.DisplayAppointment();
                Console.WriteLine("----------------");
            }
        }

        public void AddMedicalTest(MedicalTest test)
        {
            ArgumentNullException.ThrowIfNull(test);
            EnsurePatientRegistered(test.Patient);
            EnsureUniqueId(MedicalTests, test.TestId, item => item.TestId, "анализа");
            MedicalTests.Add(test);
            Console.WriteLine("Анализ добавлен!");
        }

        public void AddTest(MedicalTest test) => AddMedicalTest(test);

        public void AddPrescription(Prescription prescription)
        {
            ArgumentNullException.ThrowIfNull(prescription);
            EnsurePatientRegistered(prescription.Patient);
            if (!Doctors.Contains(prescription.Doctor))
                throw new InvalidOperationException("Врач не зарегистрирован в портале.");
            EnsureUniqueId(Prescriptions, prescription.PrescriptionId,
                item => item.PrescriptionId, "назначения");
            Prescriptions.Add(prescription);
            prescription.CreatePrescription();
        }

        public void AddPayment(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);
            EnsurePatientRegistered(payment.Patient);
            EnsureUniqueId(Payments, payment.PaymentId, item => item.PaymentId, "платежа");
            Payments.Add(payment);
        }

        public void MakePayment(Payment payment)
        {
            ArgumentNullException.ThrowIfNull(payment);
            EnsurePatientRegistered(payment.Patient);
            if (!Payments.Contains(payment))
                EnsureUniqueId(Payments, payment.PaymentId, item => item.PaymentId, "платежа");

            if (payment.MakePayment() && !Payments.Contains(payment))
                Payments.Add(payment);
        }

        public void MakePayment(int id, Patient patient, decimal amount) =>
            MakePayment(new Payment(id, patient, amount));

        public void RefundPayment(int paymentId)
        {
            Payment? payment = Payments.FirstOrDefault(item => item.PaymentId == paymentId);
            if (payment is null)
            {
                Console.WriteLine("Платёж не найден.");
                return;
            }

            payment.RefundPayment();
        }

        public void DisplayStatistics()
        {
            Console.WriteLine("\n===== СТАТИСТИКА ПОРТАЛА =====");
            Console.WriteLine("Всего пациентов: " + Patients.Count);
            Console.WriteLine("Всего врачей: " + Doctors.Count);
            Console.WriteLine("Всего записей: " + Appointments.Count);
            Console.WriteLine("Завершённых приёмов: " +
                Appointments.Count(appointment => appointment.Status == AppointmentStatus.Completed));
            Console.WriteLine("Успешно оплачено: " +
                Payments.Where(payment => payment.PaymentStatus == Payment.PaidStatus)
                    .Sum(payment => payment.Amount) + " тг");
        }

        private Patient? DisplaySearchResult(Patient? patient)
        {
            if (patient is null)
                Console.WriteLine("Пациент не найден.");
            else
                patient.DisplayInfo();
            return patient;
        }

        private void EnsurePatientRegistered(Patient patient)
        {
            if (!Patients.Contains(patient))
                throw new InvalidOperationException("Пациент не зарегистрирован в портале.");
        }

        private static void EnsureUniqueId<T>(
            IEnumerable<T> items,
            int id,
            Func<T, int> getId,
            string entityName)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID должен быть положительным.");
            if (items.Any(item => getId(item) == id))
                throw new InvalidOperationException($"Объект с таким ID ({entityName}) уже существует.");
        }
    }
}
