using Sample.SideCar.Dtos;

namespace Sample.Consumer.API.Model
{
    public class CustomerModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
