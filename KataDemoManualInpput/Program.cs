using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using KataPayDeskTerminal;

namespace KataDemoManualInpput
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string pathToCardStorage = "discountCards.xml";
                InitDiscountCards(pathToCardStorage);
                var paydeskTerminal = new PayDeskTerminal(Properties.Settings.Default.FileWithPricesPath,
                                                          Properties.Settings.Default.PricesSeparator, pathToCardStorage);
                
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

        private static void InitDiscountCards(string pathToCardStorage)
        {
            List <DiscountCard> discountCards = new List<DiscountCard>
                {
                    new DiscountCard() {CardId = "d2345", DiscountPercent = 3, PurchasesSum = 0},
                    new DiscountCard() {CardId = "e34758", DiscountPercent = 3, PurchasesSum = 235.49},
                    new DiscountCard() {CardId = "z56rt", DiscountPercent = 5, PurchasesSum = 2567.95},
                    new DiscountCard() {CardId = "eq34rt", DiscountPercent = 3, PurchasesSum = 0}
                };
            DiscountCardRepository.WriteDiscountCards(discountCards, pathToCardStorage);
        }


        private static void PurchaseProducts(PayDeskTerminal paydeskTerminal)
        {
            bool closeCheck = false;
            while (!closeCheck)
            {
                var purchaseData = Console.ReadLine();
                if (purchaseData != null && purchaseData.StartsWith("dc"))
                {
                    string discountCardId = purchaseData.Substring(3);
                    paydeskTerminal.UseDiscountCard(discountCardId);
                    Console.WriteLine("Текущий баланс на карте: " + paydeskTerminal.DiscountCard.PurchasesSum);
                    Console.WriteLine("Сумма покупки: " + paydeskTerminal.PurchasesValue);
                    continue;
                }
                if (purchaseData != "check")
                {
                    try
                    {
                        paydeskTerminal.Scan(purchaseData);
                        Console.WriteLine("Сумма покупки: " + paydeskTerminal.PurchasesValue);
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
                else
                {
                    paydeskTerminal.Calculate();
                    closeCheck = true;
                }
            }
        }

        private static void ShowPurchasesAndTotalSum(PayDeskTerminal paydeskTerminal)
        {
            foreach (var s in paydeskTerminal.PurchasesCollection)
            {
                Console.WriteLine(s.ProductName + " " + (s.BulkQuantity * s.PackPrice.Key + s.RestQuantity) + " шт., стоимость: " +
                                  s.PurchasedValue);
            }
            Console.WriteLine("Сумма покупки с учетом скидки: " + paydeskTerminal.PurchasesValue);
            Console.WriteLine("Дисконтная карта:");
            Console.WriteLine(paydeskTerminal.DiscountCard.CardId);
            Console.WriteLine("Процент скидки:" + paydeskTerminal.DiscountCard.DiscountPercent);
            Console.WriteLine("Баланс на карте после покупки:" + paydeskTerminal.DiscountCard.PurchasesSum);
            
        }
    }
}
