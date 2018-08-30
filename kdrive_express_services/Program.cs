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

namespace kdrive_express_services
{
    class Program
    {
        /*
            Set to true to use connection-oriented
            Set to false for connection-less
       */
        static bool isConnectionOriented = true;

        /*
            The address of the device that we connect
            to for the device services (Property Value Read etc)
        */
        static int address = 0xFFFF;

        static void Main(string[] args)
        {
			AccessPort accessPort = null;
			ServicePort servicePort = null;

			try
			{
				Logger.SetConsoleChannel();
				Logger.SetLevels(Logger.Level.Information);

				// USB (first available USB port)
				accessPort = new AccessPort();
				accessPort.EnumerateUsb();
				accessPort.OpenUsb(0);

				servicePort = new ServicePort(accessPort);

				/*
					Set the device services to connection-oriented or
					connection-less (depending on the value of connection_oriented)
				*/
				servicePort.SetConnectionOriented(isConnectionOriented);

				// read property value : serial number
				PropertyValueRead(servicePort);

				// write property value : programming mode
				PropertyValueWrite(servicePort);

				// switch the programming mode on
				SwitchProgMode(servicePort, true);

				// read the programming mode
				ReadProgMode(servicePort);

				// read the individual address of devices which are in programming mode
				IndividualAddressProgModeRead(servicePort);

				// write the individual address of devices which are in programming mode
				IndividualAddressProgModeWrite(servicePort);

				// switch the programming mode off
				SwitchProgMode(servicePort, false);

				// read the programming mode
				ReadProgMode(servicePort);

				// Close the service port
				servicePort.Close();

				// Close the service port
				accessPort.Close();
			}
			catch (kdrive.KdriveException exception)
			{
				Console.WriteLine(exception.Message);
			}
			finally
			{
				if (servicePort != null)
				{
					servicePort.Dispose();
				}
				if (accessPort != null)
				{
					accessPort.Dispose();
				}
				Logger.Shutdown();
			}            
        }

        /*!
	        Reads the property value from PID_SERIAL_NUMBER
	        of device with individual address "address"
        */
        static void PropertyValueRead(ServicePort servicePort)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Property Value Read");
                Logger.Log(Logger.Level.Information, "===================");
                byte[] serialNumber = servicePort.PropertyValueRead(address, 0, 11, 1, 1);
                Logger.Dump(Logger.Level.Information, "Read Serial Number: ", serialNumber);
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Property Value Read: " + e.Message);
            }
        }

        /*!
	        Writes the property value from PID_PROGMODE
	        of device with individual address "address"
        */
        static void PropertyValueWrite(ServicePort servicePort)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Property Value Write");
                Logger.Log(Logger.Level.Information, "===================");
                byte[] data = new byte[1];
                data[0] = 0;
                servicePort.PropertyValueWrite(address, 0, 54, 1, 1, data);
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Property Value Write: " + e.Message);
            }
        }


        /*!
	        Switches the programming mode off or on
	        of device with individual address "address"
        */
        static void SwitchProgMode(ServicePort servicePort, bool enable)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Switch Prog Mode");
                Logger.Log(Logger.Level.Information, "================");
                servicePort.SwitchProgMode(address, enable);
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Switch Prog Mode: " + e.Message);
            }
        }

        /*!
            Reads the programming mode
            of device with individual address "address"
        */
        static void ReadProgMode(ServicePort servicePort)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Read Prog Mode");
                Logger.Log(Logger.Level.Information, "==============");
                bool isOn = servicePort.ReadProgMode(address);
                Logger.Log(Logger.Level.Information, "Programming Mode: " + (isOn ? "on" : "off"));
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Read Prog Mode: " + e.Message);
            }
        }

        /*!
            Reads the individual addresses of devices
	        which are in programming mode.
        */
        static void IndividualAddressProgModeRead(ServicePort servicePort)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Individual Address Prog Mode Read");
                Logger.Log(Logger.Level.Information, "=================================");
                int[] addresses = servicePort.IndividualAddressProgModeRead(500);
                Logger.Log(Logger.Level.Information, "Read " + addresses.Count().ToString() + " Individual Address(es):");
                foreach(int address in addresses)
                {
                    string hex = String.Format("0x{0:X04}", address);
                    Logger.Log(Logger.Level.Information, hex);
                }
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Individual Address Prog Mode Read: " + e.Message);
            }
        }

        /*!
	        Writes the individual address to a device
	        which are in programming mode.
        */
        static void IndividualAddressProgModeWrite(ServicePort servicePort)
        {
            try
            {
                Logger.Log(Logger.Level.Information, "Individual Address Prog Mode Write");
                Logger.Log(Logger.Level.Information, "================");
                servicePort.IndividualAddressProgModeWrite(0x05F1);
                IndividualAddressProgModeRead(servicePort);
                servicePort.IndividualAddressProgModeWrite(address);
                IndividualAddressProgModeRead(servicePort);
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in Individual Address Prog Mode Write: " + e.Message);
            }
        }

    }
}
