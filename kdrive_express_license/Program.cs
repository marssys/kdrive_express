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

namespace kdrive_express_license
{
    class Program
    {
        static void Main(string[] args)
        {
			try
			{
				Logger.SetConsoleChannel();
				Logger.SetLevels(Logger.Level.Information);

				// Sets the license
				SetLicense();

				// Count USB ports
				using (AccessPort accessPort = new AccessPort())
				{
					Console.WriteLine("Found " + accessPort.EnumerateUsb() + " KNX USB Interfaces");
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

        static void SetLicense()
        {
            String license =
                "[Check]\n" +
                "Key=I1EkM2JvaE1HMEZtIlF5VnJuJn08KHRfV0AwTlEwRyk4NT55JFNvXnU6ZnpzMDI6QV42JWZ4" +
                "fHNed11aOld9WHpGXSEibnkrSDVsZU9WIkljeSQ3NWY7eT4rdEJ6XjhpNnw+OkNEIydTYWNF" +
                "RE15JVdQOno3XmtOJjZaRExwLSV8ZXxOYzlJITlhfFFdOHIjUW43SEshfSx8RWgzSipUZU5X" +
                "ZiJpSnIhRS9pJyQrOis/VVAoSHMkaV5kYGFfaFxhJj5peUtdfVBgMGh7K25bITF7OnhdeTQv" +
                "NiZlWCt4Nls3S31nLUU2LE1OXmhubGtxY2wqJHMxVDd0Tlg4OGlIUCE=\n" +
                "\n" +
                "[Customer]\n" +
                "City=84508 Burgkirchen / Alz\n" +
                "Company=WEINZIERL ENGINEERING GmbH\n" +
                "Country=Germany\n" +
                "Street=Achatz 3\n" +
                "\n" +
                "[LicenseData]\n" +
                "LicNum=kdrive-2012-05-14\n" +
                "LicVersion=71949c2a-be30-4fbe-9c96-d0a617ac6ab8\n" +
                "\n" +
                "[Limitations]" +
                "DateLimit=2012-12-31" +
                "\n" +
                "[ProgramFeatures]\n" +
                "kdriveAccess=0\n" +
                "kdriveServices=0\n";

            License.SetLicense(license);
        }
    }
}
