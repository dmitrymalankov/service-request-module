namespace srm_repositories.Models
{
    public class ServiceRequest
    {
        public string Id;
        public string BuildingCode;
        public string Description;
        public CurrentStatus CurrentStatus;
        public string CreatedBy;
        public string CreatedDate;
        public string LastModifiedBy;
        public string LastModifiedDate;
    }
}