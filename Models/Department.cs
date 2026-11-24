
using System;
using System.Collections.Generic;

namespace MedicalPortal.Models
{
    public class Department
    {
        public int DepartmentId { get; }
        public string DepartmentName { get; }
        public int Floor { get; }
        public string PhoneNumber { get; }
        public Doctor? HeadDoctor { get; private set; }
        public IReadOnlyList<Doctor> Doctors => doctors.AsReadOnly();

        private readonly List<Doctor> doctors = new();

        public Department(
            int departmentId,
            string departmentName,
            int floor,
            string phoneNumber,
            Doctor? headDoctor = null)
        {
            if (departmentId <= 0)
                throw new ArgumentOutOfRangeException(nameof(departmentId));
            if (string.IsNullOrWhiteSpace(departmentName))
                throw new ArgumentException("Название отделения обязательно.", nameof(departmentName));
            if (floor < 0)
                throw new ArgumentOutOfRangeException(nameof(floor));
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Телефон отделения обязателен.", nameof(phoneNumber));

            DepartmentId = departmentId;
            DepartmentName = departmentName.Trim();
            Floor = floor;
            PhoneNumber = phoneNumber.Trim();
            HeadDoctor = headDoctor;
            if (headDoctor is not null)
                doctors.Add(headDoctor);
        }

        public void AddDoctor(Doctor doctor)
        {
            ArgumentNullException.ThrowIfNull(doctor);
            if (!doctors.Contains(doctor))
                doctors.Add(doctor);
        }

        public void RemoveDoctor(Doctor doctor)
        {
            ArgumentNullException.ThrowIfNull(doctor);
            doctors.Remove(doctor);
            if (ReferenceEquals(HeadDoctor, doctor))
                HeadDoctor = null;
        }

        public void DisplayDoctors()
        {
            Console.WriteLine("Врачи отделения: " + DepartmentName);

            foreach (Doctor doctor in doctors)
            {
                doctor.DisplayInfo();
                Console.WriteLine("----------------");
            }
        }
    }
}
