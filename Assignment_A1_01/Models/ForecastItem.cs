namespace Assignment_A1_01.Models
{
    public class ForecastItem
    {
        public DateTime DateTime { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-20}{1,-15}{2,-15}{3,-20}{4}",
            DateTime.ToString("MM/dd/yyyy HH:mm"),
            Temperature.ToString("0.00"),
            WindSpeed.ToString("0.00"),
            Description,
            Icon);
        }
    }
}
