namespace CartingService.Services.Events
{
    public class ItemChanged : CorrelationMessage
    {
        public int Id { get; set; }

        public double Price { get; set; }

        public string Name { get; set; }
    }
}
