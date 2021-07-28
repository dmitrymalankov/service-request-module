using System;
using System.Text.Json.Serialization;

namespace srm_repositories.Models
{
    public class ServiceRequest
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("buildingCode")]
        public string BuildingCode { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("currentStatus")]
        public CurrentStatus CurrentStatus { get; set; }
        
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }
        
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
        
        [JsonPropertyName("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
        
        [JsonPropertyName("lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }
    }
}