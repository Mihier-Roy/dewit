using System;
using System.Collections.Generic;

namespace Dewit.CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> acceptedArgs = new List<string>() { "now", "later", "done", "list" };

			if (null != args[0] && acceptedArgs.Contains(args[0]))
			{
				if (args[0] == "now" || args[0] == "later")
				{
					Console.WriteLine($"Adding task : {args[1]}");
				}

				if (args[0] == "done")
				{
					Console.WriteLine($"Completing task : {args[1]}");
				}

				if (args[0] == "list")
				{
					Console.WriteLine("Showing all tasks");
				}
			}
		}
	}
}
