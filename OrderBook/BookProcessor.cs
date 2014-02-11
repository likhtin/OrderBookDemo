
using System.Collections.Generic;
using System.Linq;

namespace OrderBook
{
	
	public class BookProcessor
    {

		public List<Order> Orders { get; private set; }
		public List<Trade> Trades { get; private set; }
    

		public List<Order> OrdersCopy { get; private set; }  //for testing purpose. Should be removed


		public BookProcessor()
	    {
			Orders = new List<Order>();
			Trades = new List<Trade>();
			OrdersCopy = new List<Order>();
			
	    }

		public void PostOrder(Order order)
	    {
		    lock (Orders)
		    {
			    Orders.Add(order);
			    OrdersCopy.Add(order);
		    }
			TradeExecute(order);
	    }

		private void TradeExecute(Order order)
	    {
		 
   			//Console.WriteLine("{1} processing order {0}", order.ClOrdID, DateTime.Now.Ticks);
			
		    var tradeMatch =	from ord in Orders
			                    	where ord.Side!=order.Side
			                    	      && (order.Side==OrderSide.BUY ? ord.Price <= order.Price : ord.Price >= order.Price)
			                    	orderby ord.OrderTime  
			                    	select ord;
			lock(Orders)
		    {
			    if (tradeMatch.Any())
			    {
				    foreach (Order ord in tradeMatch)
				    {
					    Trade trade = new Trade(order.Side == OrderSide.BUY ? order.ClOrdID : ord.ClOrdID
					                            , order.Side == OrderSide.BUY ? ord.ClOrdID : order.ClOrdID
					                            , order.OrderQty > ord.OrderQty ? ord.OrderQty : order.OrderQty
					                            , order.Side == OrderSide.BUY ? order.Price : ord.Price);

					    CloseAdjustOrder(order, trade);
					    CloseAdjustOrder(ord, trade);

					    lock (Trades)
						    Trades.Add(trade);

					    if (order.OrderQty == trade.Quantity)
					    {
						    order = null;
							break;
					    }
				    }
			    }
		    }			    
		


	    }

		private void CloseAdjustOrder(Order order, Trade trade)
		{
			if (order.OrderQty == trade.Quantity)
			{
				//Console.WriteLine("Closing order {0}", order.ClOrdID);
				Orders.Remove(order);
			}
			else
			{
				//Console.WriteLine("Modifying order {0}", order.ClOrdID);
				order.UpdateQqty(order.OrderQty - trade.Quantity);
			}
		}




    }
}
