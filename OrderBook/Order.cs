using System;

namespace OrderBook
{
	public enum OrderSide
	{
		BUY,
		SELL
	}

	public class Order
	{
		public decimal Tax { get; private set; }
	    public string ClOrdID { get; private set; }
	
        public DateTime OrderTime { get; private set; }
		public OrderSide Side { get; private set; }
		public decimal OrderQty { get; private set; }
		public decimal Price { get; private set; }
	

		
		public Order(string clOrdID, OrderSide side, decimal orderQty, decimal price, DateTime orderTime)
		{
			ClOrdID = clOrdID;
			Side = side;
			OrderQty = orderQty;
			Price = price;
			OrderTime = orderTime;
		}

	    public void UpdateQqty(decimal quantity)
	    {
		    OrderQty = quantity;
	    }

    }
}
