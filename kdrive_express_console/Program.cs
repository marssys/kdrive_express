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
using System.Text;
using kdrive;
using kdrive.express.bindings;

namespace kdrive_express_console
{
    class Program
    {
        // set at least one of the following to true
        // depending on the interface that you are using
        // the default is USB
        static bool usingIP = false;
        static bool usingUSB = true;
        static bool usingTinySerial = false;

        enum GA
        {
            DPT1 = 0x1000,
            DPT2,
            DPT3,
            DPT4,
            DPT5,
            DPT6,
            DPT7,
            DPT8,
            DPT9,
            DPT10_LOCAL,
            DPT10_UTC,
            DPT10,
            DPT11_LOCAL,
            DPT11_UTC,
            DPT11,
            DPT12,
            DPT13,
            DPT14,
            DPT15,
            DPT16
        };

        static void Main(string[] args)
        {
			AccessPort accessPort = null;

			try
			{
				Logger.SetConsoleChannel();
				Logger.SetLevels(Logger.Level.Information);

				accessPort = new AccessPort();
				accessPort.TelegramEvent += new AccessPort.TelegramEventHandler(ShowTelegram);
				accessPort.NotificationEvent += new AccessPort.NotificationEventHandler(ShowNotification);

				// IP
				if (usingIP)
				{
					accessPort.OpenIp("192.168.1.204");
				}

				// USB (first available USB port)
				if (usingUSB)
				{
					accessPort.EnumerateUsb();
					accessPort.OpenUsb(0);
				}

				// Tiny Serial
				if (usingTinySerial)
				{
					accessPort.OpenTinySerial("COM14");
				}

				// show the KNX packets to/from the bus
				//accessPort.ConnectPacketTrace();

				// DPT-1 (1 bit)
				byte dataByte = Datapoint.Encode_DPT1(true);
				int bits = (int)Datapoint.Bits.DPT1;
				accessPort.GroupValueWrite((int)GA.DPT1, dataByte, bits);

				// DPT-2: 1 bit controlled
				dataByte = Datapoint.Encode_DPT2(true, true);
				bits = (int)Datapoint.Bits.DPT2;
				accessPort.GroupValueWrite((int)GA.DPT2, dataByte, bits);

				// DPT-3: 3 bit controlled
				dataByte = Datapoint.Encode_DPT3(true, 0x05);
				bits = (int)Datapoint.Bits.DPT3;
				accessPort.GroupValueWrite((int)GA.DPT3, dataByte, bits);

				// DPT-4: Character
				byte[] data = Datapoint.Encode_DPT4((byte)'A');
				accessPort.GroupValueWrite((int)GA.DPT4, data);

				// DPT-5: 8 bit unsigned value
				data = Datapoint.Encode_DPT5(0xAA);
				accessPort.GroupValueWrite((int)GA.DPT5, data);

				// DPT-6: 8 bit signed value
				data = Datapoint.Encode_DPT6(-10);
				accessPort.GroupValueWrite((int)GA.DPT6, data);

				// DPT-7: 2 byte unsigned value
				data = Datapoint.Encode_DPT7(0xAFFE);
				accessPort.GroupValueWrite((int)GA.DPT7, data);

				// DPT-8: 2 byte signed value
				data = Datapoint.Encode_DPT8(-30000);
				accessPort.GroupValueWrite((int)GA.DPT8, data);

				// DPT-9: 2 byte float value
				data = Datapoint.Encode_DPT9(12.25f);
				accessPort.GroupValueWrite((int)GA.DPT9, data);

				// DPT-10: local time
				data = Datapoint.Encode_DPT10_Local();
				accessPort.GroupValueWrite((int)GA.DPT10_LOCAL, data);

				// DPT-10: UTC time
				data = Datapoint.Encode_DPT10_UTC();
				accessPort.GroupValueWrite((int)GA.DPT10_UTC, data);

				// DPT-10: time
				data = Datapoint.Encode_DPT10(1, 11, 11, 11);
				accessPort.GroupValueWrite((int)GA.DPT10, data);

				// DPT-11: local date
				data = Datapoint.Encode_DPT11_Local();
				accessPort.GroupValueWrite((int)GA.DPT11_LOCAL, data);

				// DPT-11: UTC date
				data = Datapoint.Encode_DPT11_UTC();
				accessPort.GroupValueWrite((int)GA.DPT11_UTC, data);

				// DPT-11: date
				data = Datapoint.Encode_DPT11(2012, 3, 12);
				accessPort.GroupValueWrite((int)GA.DPT11, data);

				// DPT-12: 4 byte unsigned value
				data = Datapoint.Encode_DPT12(0xDEADBEEF);
				accessPort.GroupValueWrite((int)GA.DPT12, data);

				// DPT-13: 4 byte signed value
				data = Datapoint.Encode_DPT13(-30000);
				accessPort.GroupValueWrite((int)GA.DPT13, data);

				// DPT-14: 4 byte float value
				data = Datapoint.Encode_DPT14(2025.12345f);
				accessPort.GroupValueWrite((int)GA.DPT14, data);

				// DPT-15: Entrance access
				data = Datapoint.Encode_DPT15(1234, false, true, true, false, 10);
				accessPort.GroupValueWrite((int)GA.DPT15, data);

				// DPT-16: Character string, 14 bytes
				data = Datapoint.Encode_DPT16("Weinzierl Eng ");
				accessPort.GroupValueWrite((int)GA.DPT16, data);

				// Read the value of a Group Object
				ReadGroupObject(accessPort, 0x902);

				// trace receive telegrams (in event handler)
				Logger.Log(Logger.Level.Information, "Press any key to close");
				Console.ReadKey(true);

				accessPort.Close();
			}
			catch (kdrive.KdriveException exception)
			{
				Console.WriteLine(exception.Message);
			}
			finally
			{
				if (accessPort != null)
				{
					accessPort.Dispose();
				}
				Logger.Shutdown();
			}
        }

        /* 
            When a telegram is received we check to see if it is a group value write
	        telegram. If it is, we log the datapoint value.
	        \note The L_Data.con telegrams will in this sample also logged. 
	        If you want only see the L_Data.ind then you should check the message code. 
	        \see AccessPort.GetMessageCode
        */
        static void ShowTelegram(object sender, byte[] buffer)
        {
            try
            {
                if (AccessPort.IsGroupWrite(buffer) || 
                    AccessPort.IsGroupResponse(buffer))
                {
                    OnGroupData(buffer);
                }
            }
            catch (kdrive.KdriveException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        static void ShowNotification(object sender, AccessPortEvent e)
        {
            switch (e)
            {
                case AccessPortEvent.Opened:
                    Logger.Log(Logger.Level.Information, "Access Port Opened");
                    break;

                case AccessPortEvent.Closed:
                    Logger.Log(Logger.Level.Information, "Access Port Closed");
                    break;

                case AccessPortEvent.Terminated:
                    Logger.Log(Logger.Level.Information, "Access Port Connection Terminated");
                    break;

                case AccessPortEvent.BusConnected:
                    Logger.Log(Logger.Level.Information, "KNX Bus Connected");
                    break;

                case AccessPortEvent.BusDisconnected:
                    Logger.Log(Logger.Level.Information, "KNX Bus Disconnected");
                    break;
            }
        }

        static void OnGroupData(byte[] buffer)
        {            
            int address = AccessPort.GetDestAddress(buffer);
            byte[] data = AccessPort.GetGroupData(buffer);
            GA groupAddress = (GA)address;
            String value = "";

            switch (groupAddress)
            {
                case GA.DPT1:
                    value = String.Format("[1 Bit] {0}", Datapoint.Decode_DPT1(data));
                    break;

                case GA.DPT2:
                    bool dpt2_c = false;
                    bool dpt2_v = false;
                    Datapoint.Decode_DPT2(data, ref dpt2_c, ref dpt2_v);
                    value = String.Format("[1 Bit Controlled] Control: {0} Value: {1}", dpt2_c, dpt2_v);
                    break;

                case GA.DPT3:
                    bool dpt3_c = false;
                    byte dpt3_v = 0;
                    Datapoint.Decode_DPT3(data, ref dpt3_c, ref dpt3_v);
                    value = String.Format("[3 Bit Controlled] Control: {0} Value: {1}", dpt3_c, dpt3_v);
                    break;

                case GA.DPT4:
                    value = String.Format("[character] {0}", Datapoint.Decode_DPT4(data));
                    break;

                case GA.DPT5:
                    value = String.Format("[8 bit unsigned] {0}", Datapoint.Decode_DPT5(data));
                    break;

                case GA.DPT6:
                    value = String.Format("[8 bit signed] {0}", Datapoint.Decode_DPT6(data));
                    break;

                case GA.DPT7:
                    value = String.Format("[2 byte unsigned] {0}", Datapoint.Decode_DPT7(data));
                    break;

                case GA.DPT8:
                    value = String.Format("[2 byte signed] {0}", Datapoint.Decode_DPT8(data));
                    break;

                case GA.DPT9:
                    value = String.Format("[float] {0}", Datapoint.Decode_DPT9(data));
                    break;

                case GA.DPT10_LOCAL:
                case GA.DPT10_UTC:
                case GA.DPT10:
                    int day = 0;
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    Datapoint.Decode_DPT10(data, ref day, ref hour, ref minute, ref second);
                    value = String.Format("[time] {0} {1} {2} {3}", day, hour, minute, second);
                    break;

                case GA.DPT11:
                    int year = 0;
                    int month = 0;
                    int d = 0;
                    Datapoint.Decode_DPT11(data, ref year, ref month, ref d);
                    value = String.Format("[date] {0} {1} {2}", year, month, d);
                    break;

                case GA.DPT12:
                    value = String.Format("[4 byte unsigned] {0}", Datapoint.Decode_DPT12(data));
                    break;

                case GA.DPT13:
                    value = String.Format("[4 byte signed] {0}", Datapoint.Decode_DPT13(data));
                    break;

                case GA.DPT14:
                    value = String.Format("[4 byte float] {0}", Datapoint.Decode_DPT14(data));
                    break;

                case GA.DPT15:
                    int accessCode = 0;
                    bool error = false;
                    bool permission = false;
                    bool direction = false;
                    bool encrypted = false;
                    int index = 0;
                    Datapoint.Decode_DPT15(data, ref accessCode, ref error, ref permission,
                        ref direction, ref encrypted, ref index);
                    value = String.Format("[entrance access] {0} {1} {2} {3} {4} {5}",
                        accessCode, error, permission,
                        direction, encrypted, index);
                    break;

                case GA.DPT16:
                    value = String.Format("[character string] {0}", Datapoint.Decode_DPT16(data));
                    break;

                default:
                    Logger.Log(Logger.Level.Information, String.Format("Group Value for address: {0:x4}", address));
                    Logger.Dump(Logger.Level.Information, "Value of GroupObject", data);
                    break;
            }

            if (value != "")
            {
                String message = String.Format("{0} {1}", groupAddress, value);
                Logger.Log(Logger.Level.Information, message);
            }
        }

        static void ReadGroupObject(AccessPort accessPort, int address)
        {
            try
            {
                byte[] telegram = accessPort.ReadGroupObject(address, 1000);
                byte[] data = AccessPort.GetGroupData(telegram);
                Logger.Dump(Logger.Level.Information, "GroupValueRead response data", data);
            }
            catch (KdriveException e)
            {
                Logger.Log(Logger.Level.Error, "Error in ReadGroupObject: " + e.Message);
            }
        }
    }
}

