using CustomerGrpc;
using static Sample.SideCar.Dtos.CustomersDtos;

namespace Sample.BFF.API.Services
{
    public class CustomerServiceClient : CustomerServiceProto.CustomerServiceProtoBase
    {
        private readonly CustomerServiceProto.CustomerServiceProtoClient _client;

        public CustomerServiceClient(CustomerServiceProto.CustomerServiceProtoClient client)
        {
            _client = client;
        }

        public async Task<CustomerResponse> CreateCustomer(CreateCustomerDto customerDto)
        {
            var request = new CreateCustomerRequest
            {
                Name = customerDto.Name,
                Document = customerDto.Document,
                DocumentType = customerDto.DocumentType.ToString()
            };
            return await Task.FromResult(_client.CreateCustomerAsync(request));
        }

        public async Task<CustomerResponse> GetCustomer(string id)
        {
            var request = new GetCustomerRequest { Id = id };
            return await Task.FromResult(_client.GetCustomerAsync(request));
        }

        public async Task<CustomerResponse> UpdateCustomer(string id, UpdateCustomerDto customerDto)
        {
            var request = new UpdateCustomerRequest
            {
                Id = id,
                Name = customerDto.Name,
                Document = customerDto.Document,
                DocumentType = customerDto.DocumentType.ToString()
            };
            return await Task.FromResult(_client.UpdateCustomerAsync(request));
        }

        public async Task<bool> DeleteCustomer(string id)
        {
            var request = new DeleteCustomerRequest { Id = id };
            var response = await Task.FromResult(_client.DeleteCustomerAsync(request));
            return response.Success;
        }
    }
}

