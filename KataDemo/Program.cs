using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KataPayDeskTerminal;

namespace KataDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var properties = Properties.Settings.Default;
            var payDeskTerminal = new PayDeskTerminal(properties.FileWithPricesPath, properties.PricesSeparator);
            //
            foreach (var product in GetProductList())
            {
                payDeskTerminal.Scan(product);
            }
            Console.WriteLine(payDeskTerminal.PurchasesValue);
            Console.ReadLine();
        }

        private  static IEnumerable<string> GetProductList()
        {
            var fileContent = File.ReadAllText(Properties.Settings.Default.ProductListPath);
            var productList = fileContent.Split(Properties.Settings.Default.ProductListSeparator).ToList();
            return productList;
        }

        //add non fatal exception when no such product
        //add check that , could not be an separator
    }
}
