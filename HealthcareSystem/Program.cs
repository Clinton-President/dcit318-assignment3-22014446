using System;
using System.Collections.Generic;
using System.Linq;

// a. Generic Repository
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        return items.Remove(item);
    }
}

// b. Patient
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

// c. Prescription
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

// g. HealthSystemApp
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Ama Owusu", 29, "Female"));
        _patientRepo.Add(new Patient(2, "Kwame Mensah", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Efua Asante", 34, "Female"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Lisinopril", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("=== All Patients ===");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        Console.WriteLine($"\n=== Prescriptions for Patient ID {id} ===");
        var prescriptions = GetPrescriptionsByPatientId(id);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found.");
            return;
        }
        foreach (var p in prescriptions)
        {
            Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.MedicationName}, Date Issued: {p.DateIssued:d}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        HealthSystemApp app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(1);
    }
}