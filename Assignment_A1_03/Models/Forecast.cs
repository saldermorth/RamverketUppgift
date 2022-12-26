namespace Assignment_A1_03.Models
{
    public class Forecast
    {
        public string City { get; set; }
        public List<ForecastItem> Items { get; set; }

        public override string ToString()
        {
            return string.Format("{0,-20}{1,-15}", City, "Number of items : " + Items.Count);
        }
    }
}
