using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace Task0
{
    class EnergyProvider
    {
        static void Main(string[] args)
        {
            EnergyProvider provider = new EnergyProvider();
            Client[] clients =
            {
                new RegularClient(560),
                new RegularClient(10),
                new PreferentialClient1(90),
                new PreferentialClient1(130),
                new LimitedClient(300),
                new PreferentialClient2(40),
                new PreferentialClient1(30),
                new PreferentialClient2(270),
                new LimitedClient(250),
                new RegularClient(230),
                new LimitedClient(120),
                new PreferentialClient2(100)
            };

            #region Сортировка потребляемой энергии по убыванию
                clients = provider.DescendingSortByConsumedEnergy(clients);
                Console.WriteLine("\nArray is sorted by consumed energy");

                foreach (var item in clients)
                {
                    Console.WriteLine(item.EnergyAmount);
                }
            #endregion

            #region Сортировка по полной стоимости
                clients = provider.SortByEnergyFullPrice(clients);
                Console.WriteLine("\nArray is sorted by full price of energy");
                foreach (var item in clients)
                {
                    Console.WriteLine(item.EnergyFullPrice);
                }
            #endregion

            #region Сортирока по статусу клиента
                clients = provider.SortByClientType(clients);
                Console.WriteLine("\nArray is sorted by client status");
                foreach (var item in clients)
                {
                    Console.WriteLine(item.Type);
                }
            #endregion

            #region Полная стоимость энергии
                Console.WriteLine($"\nTotal price for all clients = {provider.TotalPaymentFromAllClients(clients)}");
            #endregion

            #region Общий размер льгот
                Console.WriteLine($"\nTotal amount of all benefits = {provider.SumOfAllBenefits(clients)}");
            #endregion

        }

        Client[] DescendingSortByConsumedEnergy(Client[] clients)
        {
            return clients.OrderByDescending(client => client.EnergyAmount).ToArray();
        }

        Client[] SortByEnergyFullPrice(Client[] clients)
        {
            return clients.OrderBy(client => client.EnergyFullPrice).ToArray();
        }

        Client[] SortByClientType(Client[] clients)
        {
            return clients.OrderBy(client => client.Type).ToArray();
        }

        double TotalPaymentFromAllClients(Client[] clients)
        {
            return clients.Select(client => client.EnergyFullPrice).Sum();
        }

        double SumOfAllBenefits(Client[] clients)
        {
            return clients.Select(client => client.EnergyAmount * client.CostPerEnergyUnit).Sum() - clients.Select(client => client.EnergyFullPrice).Sum();
        }

    }

    public enum ClientType
    {
        Regular,
        Limited,
        Preferential1,
        Preferential2
    }
    class Client
    {
        private double energyAmount;
        protected ClientType type;
        protected const double costPerEnergyUnit = 15;
        protected double tarifMultiplier;
        protected double energyFullPrice;
        public double EnergyFullPrice => energyFullPrice;
        public double EnergyAmount => energyAmount;
        public double CostPerEnergyUnit => costPerEnergyUnit;
        public ClientType Type => type;
        public Client(double energyAmount)
        {
            this.tarifMultiplier = 1;
            this.energyAmount = energyAmount;
        }

        protected virtual double CalculateFullPrice(double energyAmount, double costPerEnergyUnit, double tarifMultiplier)
        {
            return energyAmount * costPerEnergyUnit * tarifMultiplier;
        }

    }

    class RegularClient : Client
    {
        public RegularClient(double energyAmount) : base(energyAmount)
        {
            type = ClientType.Regular;
            energyFullPrice = CalculateFullPrice(energyAmount, costPerEnergyUnit, tarifMultiplier);
        }
    }
    class LimitedClient : Client
    {
        private double energyLimit = 150;

        public LimitedClient(double energyAmount) : base(energyAmount)
        {
            type = ClientType.Limited;
            tarifMultiplier = 1 / 3f;
            energyFullPrice = CalculateFullPrice(energyAmount, costPerEnergyUnit, tarifMultiplier);
        }

        protected override double CalculateFullPrice(double energyAmount, double costPerEnergyUnit, double tarifMultiplier)
        {
            return energyAmount <= energyLimit ? energyAmount * costPerEnergyUnit :
                energyLimit * costPerEnergyUnit + (energyAmount - energyLimit) * (costPerEnergyUnit + costPerEnergyUnit * tarifMultiplier);
        }

    }

    class PreferentialClient1 : Client
    {
        public PreferentialClient1(double energyAmount) : base(energyAmount)
        {
            type = ClientType.Preferential1;
            tarifMultiplier = 2 / 3f;
            energyFullPrice = CalculateFullPrice(energyAmount, costPerEnergyUnit, tarifMultiplier);
        }

    }

    class PreferentialClient2 : Client
    {
        private double amountOfFreeEnergy = 50;
        public PreferentialClient2(double energyAmount) : base(energyAmount)
        {
            type = ClientType.Preferential2;
            energyFullPrice = CalculateFullPrice(energyAmount, costPerEnergyUnit, tarifMultiplier);
        }

        protected override double CalculateFullPrice(double energyAmount, double costPerEnergyUnit, double tarifMultiplier)
        {
            return energyAmount <= amountOfFreeEnergy ? 0 :
                base.CalculateFullPrice(energyAmount - amountOfFreeEnergy, costPerEnergyUnit, tarifMultiplier);
        }
    }
}