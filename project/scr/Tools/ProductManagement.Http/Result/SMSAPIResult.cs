namespace ProductManagement.API.Http.Result
{
    public class SMSAPIResult
    {
        public Sendstatu[] sendStatus { get; set; }
        public int statu { get; set; }
        public string message { get; set; }
        public class Sendstatu
        {
            public string phoneNumber { get; set; }
            public string message { get; set; }
            public string code { get; set; }
        }
    }
}
