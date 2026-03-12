namespace EcommerceAssessment
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }

        public override string ToString()
        {
            return "OrderId: " + OrderId +
                   ", Customer: " + CustomerName +
                   ", Amount: " + Amount;
        }
    }
}
