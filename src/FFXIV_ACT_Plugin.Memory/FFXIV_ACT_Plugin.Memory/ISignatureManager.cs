using System;

namespace FFXIV_ACT_Plugin.Memory;

public interface ISignatureManager
{
	IntPtr Read(SignatureType signatureType);

	string ValidateWithMessage();
}
