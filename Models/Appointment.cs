
using System;
using System.Collections.Generic;

namespace MedicalPortal.Models
{
    public class Appointment
    {
        public int AppointmentId { get; }
        public Patient Patient { get; }
        public Doctor Doctor { get; }
        public DateTime AppointmentDate { get; }
        public TimeSpan AppointmentTime { get; }
        public int DurationMinutes { get; }
        public AppointmentStatus Status { get; private set; }

        public Appointment(
            int id,
            Patient patient,
            Doctor doctor,
            DateTime date,
            TimeSpan time,
            int durationMinutes = 30)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(patient);
            ArgumentNullException.ThrowIfNull(doctor);
            if (date == DateTime.MinValue)
                throw new ArgumentException("Дата приёма обязательна.", nameof(date));
            if (time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
                throw new ArgumentOutOfRangeException(nameof(time), "Укажите время в пределах суток.");
            if (durationMinutes <= 0 || time.Add(TimeSpan.FromMinutes(durationMinutes)) > TimeSpan.FromDays(1))
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Продолжительность приёма некорректна.");

            AppointmentId = id;
            Patient = patient;
            Doctor = doctor;
            AppointmentDate = date.Date;
            AppointmentTime = time;
            DurationMinutes = durationMinutes;
            Status = AppointmentStatus.Scheduled;
        }

        public static bool IsDoctorAvailable(
            Doctor doctor,
            DateTime date,
            TimeSpan time,
            IEnumerable<Appointment> appointments,
            int durationMinutes = 30)
        {
            ArgumentNullException.ThrowIfNull(doctor);
            ArgumentNullException.ThrowIfNull(appointments);
            DateTime proposedStart = date.Date.Add(time);
            DateTime proposedEnd = proposedStart.AddMinutes(durationMinutes);

            foreach (Appointment appointment in appointments)
            {
                if (appointment.Doctor.DoctorId == doctor.DoctorId &&
                    appointment.Status != AppointmentStatus.Cancelled)
                {
                    DateTime existingStart = appointment.AppointmentDate.Add(appointment.AppointmentTime);
                    DateTime existingEnd = existingStart.AddMinutes(appointment.DurationMinutes);
                    if (proposedStart < existingEnd && existingStart < proposedEnd)
                    {
                        Console.WriteLine("Врач уже занят на указанное время!");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsDoctorAvailable(DateTime date, IEnumerable<Appointment> appointments)
        {
            return IsDoctorAvailable(Doctor, date.Date, date.TimeOfDay, appointments, DurationMinutes);
        }

        public void CreateAppointment()
        {
            Status = AppointmentStatus.Scheduled;
            Console.WriteLine("Запись создана!");
        }

        public void CancelAppointment()
        {
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Завершённую запись нельзя отменить.");

            Status = AppointmentStatus.Cancelled;
            Console.WriteLine("Запись отменена.");
        }

        public void ConfirmAppointment()
        {
            if (Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Отменённую запись нельзя подтвердить.");
            if (Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Завершённую запись нельзя подтвердить.");

            Status = AppointmentStatus.Confirmed;
            Console.WriteLine("Запись подтверждена.");
        }

        public void CompleteAppointment()
        {
            if (Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Отменённую запись нельзя завершить.");

            Status = AppointmentStatus.Completed;
            Console.WriteLine("Приём завершён.");
        }

        public void DisplayAppointment()
        {
            Console.WriteLine("Номер записи: " + AppointmentId);
            Console.WriteLine("Пациент: " + Patient.FullName);
            Console.WriteLine("Врач: " + Doctor.FullName);
            Console.WriteLine("Дата: " + AppointmentDate.ToShortDateString());
            Console.WriteLine("Время: " + AppointmentTime);
            Console.WriteLine("Длительность: " + DurationMinutes + " минут");
            Console.WriteLine("Статус: " + Status);
        }
    }
}
