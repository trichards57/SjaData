using SjaData.Server.Model.Patient;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class PatientService : IPatientService
{
    public Task AddAsync(NewPatient patient)
    {
        throw new NotImplementedException();
    }

    public Task<PatientCount> CountAsync(PatientQuery query)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
