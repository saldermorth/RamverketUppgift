namespace Assignment_A1_03.Exceptions
{
    internal class NonExistingCityException : Exception
    {
        public NonExistingCityException()
        {
        }
        public NonExistingCityException(string message) : base(message)
        {

        }
        public NonExistingCityException(string message, HttpResponseMessage response) : base(message)
        {

        }

        public NonExistingCityException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
