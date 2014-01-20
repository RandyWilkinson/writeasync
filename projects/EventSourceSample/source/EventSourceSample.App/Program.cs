﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EventSourceSample
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Task task = RunAsync(new Uri("net.pipe://localhost/Calculator"), cts.Token);

                Console.WriteLine("Press ENTER to stop.");
                Console.ReadLine();

                cts.Cancel();
                task.Wait();

                Console.WriteLine("Done.");
            }
        }

        private static Task RunAsync(Uri address, CancellationToken token)
        {
            Task clientTask = RunClientAsync(address, token);
            Task serviceTask = RunServiceAsync(address, token);

            return Task.WhenAll(clientTask, serviceTask);
        }

        private static async Task RunServiceAsync(Uri address, CancellationToken token)
        {
            CalculatorServiceHost host = new CalculatorServiceHost(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), address);
            await host.OpenAsync();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5.0d), token);
                    await host.RecycleAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }

            await host.CloseAsync();
        }

        private static async Task RunClientAsync(Uri address, CancellationToken token)
        {
            ClientEventSource eventSource = ClientEventSource.Instance;

            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.OpenTimeout = TimeSpan.FromSeconds(1.0d);
            binding.SendTimeout = TimeSpan.FromSeconds(1.0d);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(1.0d);
            binding.CloseTimeout = TimeSpan.FromSeconds(1.0d);

            CalculatorProxy proxy = new CalculatorProxy(binding, new EndpointAddress(address), c => WrapClient(c, eventSource));
            await proxy.OpenAsync();

            Random random = new Random();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await proxy.InvokeAsync(c => InvokeRandomAsync(random, c));
                }
                catch (Exception)
                {
                }

                await Task.Delay(TimeSpan.FromMilliseconds(250.0d));
            }

            await proxy.CloseAsync();
        }

        private static ICalculatorClientAsync WrapClient(ICalculatorClientAsync lower, ClientEventSource eventSource)
        {
            ICalculatorClientAsync middle = new CalculatorClientWithEvents(lower, eventSource);
            return new CalculatorClientWithActivity(middle, eventSource, Guid.NewGuid());
        }

        private static async Task<double> InvokeRandomAsync(Random random, ICalculatorClientAsync client)
        {
            double result;
            switch (random.Next(3))
            {
                case 0:
                    result = await client.AddAsync(random.NextDouble(), random.NextDouble());
                    break;
                case 1:
                    result = await client.SubtractAsync(random.NextDouble(), random.NextDouble());
                    break;
                case 2:
                    result = await client.SquareRootAsync(random.NextDouble() - random.NextDouble());
                    break;
                default:
                    throw new InvalidOperationException("Should never happen.");
            }

            return result;
        }
    }
}
