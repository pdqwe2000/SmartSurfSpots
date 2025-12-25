using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SmartSurfSpots.SoapService.Contracts
{
    [ServiceContract]
    public interface ISoapDataService
    {
        // User Operations
        [OperationContract]
        Task<SoapUserResponse> GetUserById(int id);

        [OperationContract]
        Task<SoapUserResponse> GetUserByEmail(string email);

        [OperationContract]
        Task<List<SoapUserResponse>> GetAllUsers();

        // Spot Operations
        [OperationContract]
        Task<SoapSpotResponse> GetSpotById(int id);

        [OperationContract]
        Task<List<SoapSpotResponse>> GetAllSpots();

        [OperationContract]
        Task<SoapSpotResponse> CreateSpot(SoapCreateSpotRequest request);

        [OperationContract]
        Task<SoapSpotResponse> UpdateSpot(SoapUpdateSpotRequest request);

        [OperationContract]
        Task<bool> DeleteSpot(int id);

        // CheckIn Operations
        [OperationContract]
        Task<SoapCheckInResponse> CreateCheckIn(SoapCreateCheckInRequest request);

        [OperationContract]
        Task<List<SoapCheckInResponse>> GetCheckInsBySpot(int spotId);

        [OperationContract]
        Task<List<SoapCheckInResponse>> GetCheckInsByUser(int userId);
    }

    // Data Transfer Objects para SOAP
    public class SoapUserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class SoapSpotResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }

    public class SoapCreateSpotRequest
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int CreatedBy { get; set; }
    }

    public class SoapUpdateSpotRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }

    public class SoapCheckInResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SpotId { get; set; }
        public string SpotName { get; set; }
        public string DateTime { get; set; }
        public string Notes { get; set; }
    }

    public class SoapCreateCheckInRequest
    {
        public int UserId { get; set; }
        public int SpotId { get; set; }
        public string Notes { get; set; }
    }
}