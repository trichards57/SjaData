using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IVehicleService
{
    IAsyncEnumerable<VehicleTypeStatus> GetNationalStatus();
    IAsyncEnumerable<VehicleSettings> GetVehicleSettings();
    Task<VehicleSettings> GetVehicleSettingsAsync(int id);
    IAsyncEnumerable<VorStatus> GetVorStatus(Region region);
    Task PostVehicleSettingsAsync(UpdateVehicleSettings settings);
}
