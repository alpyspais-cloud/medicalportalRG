
using System;

namespace MedicalPortal.Models
{
    public class MedicalRecord
    {
        public int RecordId { get; }
        public Patient Patient { get; private set; }
        public int PatientId { get; }
        public string Diagnosis { get; private set; } = "";
        public string MedicalHistory { get; private set; } = "";
        public DateTime CreationDate { get; }

        public MedicalRecord(int id, Patient patient)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(patient);

            RecordId = id;
            Patient = patient;
            PatientId = patient.PatientId;
            Diagnosis = "Не установлен";
            MedicalHistory = "";
            CreationDate = DateTime.Now;
        }

        public void AddDiagnosis(string diagnosis)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Диагноз не может быть пустым.", nameof(diagnosis));
            Diagnosis = diagnosis.Trim();
            Console.WriteLine("Диагноз добавлен.");
        }

        public void UpdateHistory(string history)
        {
            if (history is null)
                throw new ArgumentNullException(nameof(history));
            MedicalHistory = history.Trim();
            Console.WriteLine("История болезни обновлена.");
        }

        public void DisplayRecord()
        {
            Console.WriteLine("Медицинская карта №" + RecordId);
            Console.WriteLine("Пациент: " + Patient.FullName);
            Console.WriteLine("Диагноз: " + Diagnosis);
            Console.WriteLine("История болезни: " + MedicalHistory);
            Console.WriteLine("Дата создания: " + CreationDate);
        }
    }
}
