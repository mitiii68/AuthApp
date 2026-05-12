namespace AuthApp.Models
{
    public class ContractDocument
    {
        public int Id { get; set; }

        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public int FileDocumentId { get; set; }
        public FileDocuments? FileDocument { get; set; }



    }


    

}
