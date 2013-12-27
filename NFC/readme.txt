NDEF Library for Proximity APIs (NFC)
http://ndef.codeplex.com/
-------------------------------------------------------------------------------

Easily parse and create NDEF records with C# and the Windows Proximity APIs (NFC).


Overview
-------------------------------------------------------------------------------
The Proximity APIs for NFC (Near Field Communication) in the Windows 8 platform are compatible to NDEF (NFC Data Exchange Format) messages, which are used on NFC tags and to send data between two devices.

While the Proximity APIs include basic support for URIs, they lack support for more in-depth control over the NDEF records, as well as additional standardized record types.

This NDEF Library provides a set of classes that enable you to easily work with NDEF records on top of the Windows Proximity APIs.

Integrate the library into your Windows (Phone) 8 project using the NuGet package manager of Visual Studio!


Reusable NDEF classes
-------------------------------------------------------------------------------

- Parse NDEF message & records from raw byte arrays 
- Extract all information from the bits & bytes contained in the record 
- Create standard compliant records just by providing your data 
- Supports fully standardized basic record types:
  - Smart Poster, URI, Text records 
- Smart URI class: automatically represents itself as the smallest possible NDEF type (URI or Smart Poster), depending on supplied data 
- Convenience classes for: 
  - LaunchApp tags - launch a Windows (Phone) app just by tapping a tag
  - Nokia Accessories tags - let the user choose an app to launch on his Nokia Lumia Windows Phone 8 device
  - WpSettings tags - launch a settings page on Windows Phone 8 (e.g., Bluetooth settings, flight mode). Actually modifying these settings is not allowed by the security model of Windows Phone
  - Android Application Record (AAR) tags - launch an Android app by tapping a tag
  - Geo tags - longitude & latitude of a place, using different Geo URI schemes (more details) 
  - Social tags - linking to social networks like Twitter, Facebook, Foursquare or Skype 
  - SMS tags - defining number and body of the message
  - Mailto tags - sending email messages with recipient address and optional subject and body
  - Telephone call tags - defining the number to call
  - NearSpeak tags - store voice messages on NFC tags, using the custom URI scheme as defined by the NearSpeak app: http://www.nearspeak.at/
- Records check their contents for validity according to standards
- Can throw NdefException in case of content validity issues, with translatable messages defined in a resource file
- Fully documented source code, following Doxygen standards


Usage example
-------------------------------------------------------------------------------

** Reading & parsing a Smart Poster

private void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
{
    // Parse raw byte array to NDEF message
    var rawMsg = message.Data.ToArray();
	var ndefMessage = NdefMessage.FromByteArray(rawMsg);

	// Loop over all records contained in the NDEF message
	foreach (NdefRecord record in ndefMessage) 
	{
		Debug.WriteLine("Record type: " + Encoding.UTF8.GetString(record.Type, 0, record.Type.Length));
		// Go through each record, check if it's a Smart Poster
		if (record.CheckSpecializedType(false) == typeof (NdefSpRecord))
		{
			// Convert and extract Smart Poster info
			var spRecord = new NdefSpRecord(record);
			Debug.WriteLine("URI: " + spRecord.Uri);
			Debug.WriteLine("Titles: " + spRecord.TitleCount());
			Debug.WriteLine("1. Title: " + spRecord.Titles[0].Text);
			Debug.WriteLine("Action set: " + spRecord.ActionInUse());
		}
	}
}


** Writing a Smart Poster

// Initialize Smart Poster record with URI, Action + 1 Title
var spRecord = new NdefSpRecord {
                  Uri = "http://www.nfcinteractor.com", 
                  NfcAction = NdefSpActRecord.NfcActionType.DoAction };
spRecord.AddTitle(new NdefTextRecord { 
                  Text = "Nfc Interactor", LanguageCode = "en" });

// Add record to NDEF message
var msg = new NdefMessage { spRecord };

// Publish NDEF message to a tag
// AsBuffer(): add -> using System.Runtime.InteropServices.WindowsRuntime;
_device.PublishBinaryMessage("NDEF:WriteTag", msg.ToByteArray().AsBuffer());

// Alternative: send NDEF message to another NFC device
_device.PublishBinaryMessage("NDEF", msg.ToByteArray().AsBuffer());


Installation
-------------------------------------------------------------------------------
Easiest is to use the NuGet package manager in Visual Studio to integrate the portable library with your project:

Prerequisites
- Visual Studio 2012 or higher
  Required for using portable class libraries that target Windows Phone 8 & Windows Store apps.
- If using Visual Studio 2012, ensure you have Nuget version >= 2.1
  The Visual Studio 2012 Update 1 doesn't automatically update NuGet as well.
  Please go to "Tools" -> "Extensions and Updates..." to search for updates and install all available updates in this dialog.
  If you have the initial version of NuGet installed, you will *always* get an error message about incompatible platforms when installing the NDEF library, as Windows Phone 8 as a target was not understood by the old NuGet manager.

Installation
- Tools -> Library Package Manager -> Manage NuGet Packages for Solution...
- Search "Online" for "NDEF"
- Install the "NDEF Library for Proximity APIs (NFC)" 

More instructions: http://ndef.codeplex.com/documentation

You can also download the complete portable library project from the source control server of this project, and build the library yourself, or directly integrate the relevant class files.


Version History
-------------------------------------------------------------------------------
1.1.0.0 - July 2013
- SMS handling improved: allows wrong sms:// scheme, parses URLs without body text and/or number
- Social record adds Google+ and the FourSquare protocol
- Adds dictionary of available Nokia Accessories including their product names
- NearSpeak record now understands cloud-based tags
- Improved comments for NDEF message, removed debug output


Status & Roadmap
-------------------------------------------------------------------------------
The NDEF library is classified as stable release and is in use in several projects, most importantly Nfc Interactor for Windows Phone (http://www.nfcinteractor.com/).

Any open issues as well as planned features are tracked online:
https://ndef.codeplex.com/workitem/list/basic


Related Information
-------------------------------------------------------------------------------
Parts of this library are based on the respective code of the Connectivity Module of Qt Mobility (NdefMessage, NdefRecord, NdefUriRecord and NdefTextRecord classes. Original source code: http://qt.gitorious.org/qt-mobility).

More information about the library:
http://www.nfcinteractor.com/ndef-library/

Library homepage:
http://ndef.codeplex.com/

