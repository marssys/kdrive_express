//
// Copyright (c) 2002-2017 WEINZIERL ENGINEERING GmbH
// All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
// SHALL THE COPYRIGHT HOLDERS BE LIABLE FOR ANY DAMAGES OR OTHER LIABILITY,
// WHETHER IN CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kdrive;
using kdrive.express.bindings;

namespace kdrive_express_tunneling_enumeration
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Logger.SetConsoleChannel();
				Logger.SetLevels(Logger.Level.Information);

				using (AccessPort accessPort = new AccessPort())
				{
					SearchTunnelingDevices(accessPort);

					accessPort.Close();
				}
			}
			catch (kdrive.KdriveException exception)
			{
				Console.WriteLine(exception.Message);
			}
			finally
			{
				Logger.Shutdown();
			}
		}

		/// <summary>
		/// Searchs the KNXnet/IP Tunneling devices and prints the descriptions.
		/// </summary>
		/// <param name="accessPort">access port</param>
		static void SearchTunnelingDevices(AccessPort accessPort)
		{
			Console.WriteLine("Enumerating KNX IP Tunneling Interfaces");
			Console.WriteLine("========================================");

			List<KnxNetIpDeviceDescription> devices = accessPort.EnumerateIpTunneling();

			Console.WriteLine("Found {0} device(s)", devices.Count);

			foreach (KnxNetIpDeviceDescription desc in devices)
			{
				Console.WriteLine("");
				Console.WriteLine("Name: {0}", desc.GetDeviceName());
				Console.WriteLine("{0} on {1}", desc.GetIpAddress(), desc.GetNetworkInterface());
				Console.WriteLine("MAC: {0}", BitConverter.ToString(desc.GetMacAddress()));
				Console.WriteLine("Serial Number: {0}", BitConverter.ToString(desc.GetSerialNumber()));
				Console.WriteLine("Individual Address: 0x{0:X04}", desc.GetIndividualAddress());
				Console.WriteLine("Programming Mode: {0}", desc.IsProgrammingModeOn() ? "on" : "off");
				Console.WriteLine("");
			}

			Console.WriteLine("");
		}
	}
}