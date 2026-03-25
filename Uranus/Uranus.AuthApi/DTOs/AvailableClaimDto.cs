namespace Uranus.AuthApi.DTOs
{
    public class AvailableClaimDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public int? ParentClaimId { get; set; }
        //public string ParentType { get; set; }
        //public string ParentValue { get; set; }
        //public string ParentDescription { get; set; }
    }
}