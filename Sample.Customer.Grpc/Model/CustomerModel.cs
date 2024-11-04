using Sample.SideCar.Dtos;

namespace Sample.Customer.Grpc.Model
{
    public class CustomerModel
    {
        public string Id { get; set; } = Ulid.NewUlid().ToString();
        public string Name { get; set; }
        public string Document { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
