using System;
using System.Diagnostics;

namespace Bitcoin
{
    public class Node
    {
        public void Run()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/usr/local/bin/bitcoind"}; 
                Process proc = new Process() { StartInfo = startInfo, };
                proc.Start();
            }
            catch(Exception)
            {
            }
        }
        public void Down()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/usr/local/bin/bitcoin-cli", Arguments = "stop"}; 
                Process proc = new Process() { StartInfo = startInfo, };
                proc.Start();
            }
            catch(Exception)
            {
            }
        }
    }
}
