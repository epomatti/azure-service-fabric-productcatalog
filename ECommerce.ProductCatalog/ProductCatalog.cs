﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _repo;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProduct(Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _repo.GetAllProducts();
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            return await _repo.GetProduct(productId);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();

        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repo = new ServiceFabricProductRepository(this.StateManager);
            IEnumerable<Product> all = await _repo.GetAllProducts();
            if(all.Count() == 0)
            {
                var product1 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Dell Monitor",
                    Description = "Computer Monitor",
                    Price = 500,
                    Availability = 100
                };

                var product2 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Surface Book",
                    Description = "Microsoft's Latest Laptop, i7 CPU, 1Tb SSD",
                    Price = 2200,
                    Availability = 15
                };

                var product3 = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Arc Touch Mouse",
                    Description = "Computer Mouse, bluetooth, requires 2 AAA batteries",
                    Price = 60,
                    Availability = 30
                };

                await _repo.AddProduct(product1);
                await _repo.AddProduct(product2);
                await _repo.AddProduct(product3);
            }
             
        }
    }
}
