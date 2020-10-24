using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var colvirReport = generateCollection(0, 45000);
            colvirReport.AddRange(generateCollection(45000, 50000));
            
            var abrReport = generateCollection(0, 45000);
            colvirReport.AddRange(generateCollection(50000, 55000));
            
            var res = new Dictionary<String, PaymentDiff>();

            Console.WriteLine($"Start {DateTime.Now}");
            foreach (var payment in colvirReport)
            {
                res.AddOrUpdatePayment(payment, false);
            }
            Console.WriteLine($"After colvir {DateTime.Now}");
            foreach (var payment in abrReport)
            {
                res.AddOrUpdatePayment(payment, true);
            }
            Console.WriteLine($"After Abr {DateTime.Now}");
            Console.WriteLine($"res size {res.Count}");
        }

        static List<Payment> generateCollection(int start, int finish)
        {
            Random rnd = new Random();
            var paymens = new List<Payment>();

            for (int i = start; i < finish; i++)
            {
                paymens.Add(new Payment()
                {
                    name = rnd.Next(0, 999999999).ToString(),
                    time = (new TimeSpan(12, 0, 0))
                        .Add(new TimeSpan(rnd.Next(-779, 779))),
                    id = i.ToString()
                });
            }

            return paymens;
        }
    }

    public class Payment
    {
        public String name { set; get; }
        
        public TimeSpan time { set; get; }
        
        public String id { set; get; }
    }

    public class PaymentDiff
    {
        public Payment ColvirPayment { set; get; }
        
        public Payment AbrPayment { set; get; }
    }

    public static class DictExt
    {
        public static void AddOrUpdatePayment(
            this Dictionary<String, PaymentDiff> payments, 
            Payment payment,
            bool isAbr)
        {
            if (payments.ContainsKey(payment.id))
            {
                if (isAbr)
                {
                    payments[payment.id].AbrPayment = payment;
                }
                else
                {
                    payments[payment.id].ColvirPayment = payment;
                }
            }
            else
            {
                if (isAbr)
                {
                    payments.Add(payment.id, new PaymentDiff()
                    {
                        AbrPayment = payment
                    });
                }
                else
                {
                    payments.Add(payment.id, new PaymentDiff()
                    {
                        ColvirPayment = payment
                    });
                }
            }
        }
    }
}