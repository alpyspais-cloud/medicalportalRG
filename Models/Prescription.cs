
using System;

namespace MedicalPortal.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; }
        public Patient Patient { get; private set; }
        public Doctor Doctor { get; private set; }
        public string MedicationName { get; private set; } = "";
        public string Dosage { get; private set; } = "";
        public int Duration { get; private set; }

        public Prescription(
            int id,
            Patient patient,
            Doctor doctor,
            string medication,
            string dosage,
            int duration)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(patient);
            ArgumentNullException.ThrowIfNull(doctor);

            PrescriptionId = id;
            Patient = patient;
            Doctor = doctor;
            (MedicationName, Dosage, Duration) = ValidatePrescription(medication, dosage, duration);
        }

        public void CreatePrescription()
        {
            Console.WriteLine("Назначение создано.");
        }

        public void UpdatePrescription(
            string medication,
            string dosage,
            int duration)
        {
            (MedicationName, Dosage, Duration) = ValidatePrescription(medication, dosage, duration);
        }

        public void DisplayPrescription()
        {
            Console.WriteLine("Назначение №" + PrescriptionId);
            Console.WriteLine("Пациент: " + Patient.FullName);
            Console.WriteLine("Врач: " + Doctor.FullName);
            Console.WriteLine("Препарат: " + MedicationName);
            Console.WriteLine("Дозировка: " + Dosage);
            Console.WriteLine("Продолжительность: " + Duration + " дней");
        }

        private static (string Medication, string Dosage, int Duration) ValidatePrescription(
            string medication,
            string dosage,
            int duration)
        {
            if (string.IsNullOrWhiteSpace(medication))
                throw new ArgumentException("Название препарата обязательно.", nameof(medication));
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Дозировка обязательна.", nameof(dosage));
            if (duration <= 0)
                throw new ArgumentOutOfRangeException(nameof(duration), "Продолжительность должна быть больше нуля.");
            return (medication.Trim(), dosage.Trim(), duration);
        }
    }
}
