using System;
using KataPayDeskTerminal;

namespace KataDemoManualInpput
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var paydeskTerminal = new PayDeskTerminal(Properties.Settings.Default.FileWithPricesPath,
                                                          Properties.Settings.Default.PricesSeparator);
                PurchaseProducts(paydeskTerminal);
                ShowPurchasesAndTotalSum(paydeskTerminal);
            }
            catch (DuplicateProductException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (PricesValidationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static void PurchaseProducts(PayDeskTerminal paydeskTerminal)
        {
            bool closeCheck = false;
            while (!closeCheck)
            {
                var productName = Console.ReadLine();
                if (productName != "check")
                {
                    try
                    {
                        paydeskTerminal.Scan(productName);
                        Console.WriteLine("Сумма покупок: " + paydeskTerminal.PurchasesValue);
                    }
                    catch (ProductNotExistException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("some Exception occures, debug and look at stacktrace");
                    }
                }
                else closeCheck = true;
            }
        }

        private static void ShowPurchasesAndTotalSum(PayDeskTerminal paydeskTerminal)
        {
            foreach (var s in paydeskTerminal.PurchasesCollection)
            {
                Console.WriteLine(s.ProductName + " " + (s.BulkQuantity * s.PackPrice.Key + s.RestQuantity) + " шт., стоимость: " +
                                  s.PurchasedValue);
            }
            Console.WriteLine("Общая сумма покупок: " + paydeskTerminal.PurchasesValue);
        }
    }
}
