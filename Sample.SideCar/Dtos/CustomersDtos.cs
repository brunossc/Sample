namespace Sample.SideCar.Dtos
{
    public class CustomersDtos
    {
        public record CreateCustomerDto(string Name, string Document, DocumentType DocumentType);
        public record UpdateCustomerDto(string Id, string Name, string Document, DocumentType DocumentType);
        public record CustomerDto(string Id, string Name, string Document, DocumentType DocumentType);
    }

    public enum DocumentType
    {
        RG,
        CPF
    }
}
