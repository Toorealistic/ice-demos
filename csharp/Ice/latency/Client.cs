// **********************************************************************
//
// Copyright (c) 2003-2017 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using Demo;
using System;
using System.Reflection;

public class Client
{
    public class App : Ice.Application
    {
        public override int run(string[] args)
        {
            if(args.Length > 0)
            {
                Console.Error.WriteLine(appName() + ": too many arguments");
                return 1;
            }

            PingPrx ping = PingPrxHelper.checkedCast(communicator().propertyToProxy("Ping.Proxy"));
            if(ping == null)
            {
                Console.Error.WriteLine("invalid proxy");
                return 1;
            }
            
            //
            // A method needs to be invoked thousands of times before the JIT compiler
            // will convert it to native code. To ensure an accurate latency measurement,
            // we need to "warm up" the JIT compiler.
            //
            {
                int reps = 20000;
                Console.Error.Write("warming up the JIT compiler...");
                Console.Error.Flush();
                for(int i = 0; i < reps; i++)
                {
                    ping.ice_ping();
                }
                Console.Error.WriteLine("ok");
            }
            
            var tv1 = DateTime.Now;
            int repetitions = 100000;
            Console.Out.WriteLine("pinging server " + repetitions + " times (this may take a while)");
            for (int i = 0; i < repetitions; i++)
            {
                ping.ice_ping();
            }
            
            var total = (DateTime.Now - tv1).TotalMilliseconds;
            var perPing = total / repetitions;
            
            Console.Out.WriteLine("time for " + repetitions + " pings: " + total + "ms");
            Console.Out.WriteLine("time per ping: " + perPing + "ms");
            
            return 0;
        }
    }

    public static int Main(string[] args)
    {
        var app = new App();
        return app.main(args, "config.client");
    }
}
