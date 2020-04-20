namespace parking_enforcement_service.Models
{
    public class EventCodeConfiguration
    {
        public EventCodeConfiguration(int shielded, int nonShielded)
        {
            Shielded = shielded;
            NonShielded = nonShielded;
        }

        public int Shielded { get; set; }
        public int NonShielded { get; set; }
    }
}