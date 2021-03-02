using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using log4net.Config;

[assembly: AssemblyTitle("EjemploMFile")]
[assembly: AssemblyDescription("Ejemplo integración M-Files")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Diego Cabezas ® COY")]
[assembly: AssemblyProduct("Ejemplo integración M-Files")]
[assembly: AssemblyCopyright("Copyright © Diego Cabezas 2020")]
[assembly: AssemblyTrademark("© 2007 - 2020 Diego Cabezas™. Todos los derechos reservados.")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("dc831ec1-c7e3-4b17-9016-60a72beeb184")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
[assembly: XmlConfigurator(Watch = true)]
[assembly: SuppressIldasm]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]