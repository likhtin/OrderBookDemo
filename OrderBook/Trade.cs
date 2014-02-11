

namespace OrderBook
{
	public class Trade
	{
		public string OrderIdBuy { get; private set; }
		public string OrderIdSell { get; private set; }
		public decimal Quantity { get; private set; }
		public decimal Price { get; private set; }

		public Trade(string orderIdBuy, string orderIdSell, decimal quanity, decimal price)
		{
			OrderIdBuy = orderIdBuy;
			OrderIdSell = orderIdSell;
			Quantity = quanity;
			Price = price;
		}


	}
}
