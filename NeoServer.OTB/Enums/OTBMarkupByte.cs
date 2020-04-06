using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.OTB.Enums
{
	public enum OTBMarkupByte : byte
	{
		Escape = 0xFD,
		Start = 0xFE,
		End = 0xFF
	}
}
