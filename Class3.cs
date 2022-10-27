using System;
using System.Diagnostics;

public class Class1
{
	public Class1()
	{
		//открыть сайт в браузере
		Process.Start(new ProcessStartInfo { FileName = @"Адрес сайта", UseShellExecute = true });

	}
}
