using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IVehicleService
{
    IAsyncEnumerable<VehicleSettings> GetVehicleSettings();
}
