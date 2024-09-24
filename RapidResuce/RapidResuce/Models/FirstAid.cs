using System.ComponentModel;

namespace RapidResuce.Models
{
    public class FirstAid : BaseEntity
    {
        [DisplayName("Symptoms")]
        public string symptoms { get; set; }

        [DisplayName("Precautions")]
        public string FirstAidDetail { get; set; }
    }
}
