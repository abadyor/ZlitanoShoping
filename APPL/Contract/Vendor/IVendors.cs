
using APPL.DTOs.Request.Vendor;

using APPL.DTOs.Response.Vendor;
using APPL.DTOS.Response;

namespace APPL.Contract.Vendor
{
    public interface IVendors
    {
        Task<GeneralRespond> CreateAsync(VendorRequestCreateDTO vendorRequestCreateDTO);
        Task<GeneralRespond> DeleteAsync(int vendorId);
        Task<List<VendorResponseDTO>> GetAllAsync();
        Task<List<VendorResponseDTO>> GetAllStoreCodesAsync();
        Task<VendorResponse_Stores_DTO> GetByIdAsync(int vendorId);
        Task<GeneralRespond> VendorLoginAsync(VendorLoginDTO loginDto);
        Task<GeneralRespond> VendorLogCode_ChekAsync(string Username, string Log_Code);
        Task<VendorResponse_Stores_DTO> GetByCodeAsync(string vendorCode);
        Task<GeneralRespond> UpdateLogCodeAsync(VendorLogCode_UpdateDTO vendorLogCode_UpdateDTO);

        public Task<string> GenerateCodeAndSendEmailAndGetCode(string recipientEmail);
        public Task<string> GenerateCodeAndSendEmail(string recipientEmail);

        public Task<string> updateonefild(string code);
        public Task<VendorResponseDTO?> VendorUsernamePasswordAsync(string username, string password);


        // public Task SendEmailAsyncAnotherFunction(string title, string body);

        public Task<VendorResponseDTO> GetVendorByUserNamePasswordLogUser(string username, string password, string loguser);


    }
}
