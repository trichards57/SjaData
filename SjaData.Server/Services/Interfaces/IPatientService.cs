using SjaData.Server.Model.Patient;

namespace SjaData.Server.Services.Interfaces;

public interface IPatientService
{
    Task AddAsync(NewPatient patient);
    Task<PatientCount> CountAsync(PatientQuery query);
    Task DeleteAsync(int id);
    Task<DateTimeOffset> GetLastModifiedAsync();
}
