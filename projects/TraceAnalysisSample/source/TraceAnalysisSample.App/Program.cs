﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace TraceAnalysisSample
{
    using System;

    internal sealed class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide an ETL file name.");
                return 1;
            }

            TraceReader reader = new TraceReader(args[0]);
            reader.ReadAsync().Wait();

            return 0;
        }
    }
}
