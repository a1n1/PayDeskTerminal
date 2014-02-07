using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataPayDeskTerminal
{
    public interface IPayDeskTerminal
    {
        void SetPrices();
        void Scan(string productName);
        double Calculate();
        void UseDiscountCard(string cardId);
    }
}
