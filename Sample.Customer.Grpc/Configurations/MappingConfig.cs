using Mapster;
using Sample.Customer.Grpc.Model;
using static Sample.SideCar.Dtos.CustomersDtos;

namespace Sample.Customer.Grpc.Configurations
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<CustomerDto, CustomerModel>.NewConfig();
            TypeAdapterConfig<CreateCustomerDto, CustomerModel>.NewConfig();
            TypeAdapterConfig<CustomerModel, CustomerDto>.NewConfig();
        }
    }
}
