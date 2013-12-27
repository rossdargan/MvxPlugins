/****************************************************************************
**
** Copyright (C) 2012 - 2013 Andreas Jakl.
** All rights reserved.
**
** Extension to the NDEF handling classes.
**
** Created by Andreas Jakl (2012).
** More information: http://ndef.codeplex.com/
**
** GNU Lesser General Public License Usage
** This file may be used under the terms of the GNU Lesser General Public
** License version 2.1 as published by the Free Software Foundation and
** appearing in the file LICENSE.LGPL included in the packaging of this
** file. Please review the following information to ensure the GNU Lesser
** General Public License version 2.1 requirements will be met:
** http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html.
**
** GNU General Public License Usage
** Alternatively, this file may be used under the terms of the GNU General
** Public License version 3.0 as published by the Free Software Foundation
** and appearing in the file LICENSE.GPL included in the packaging of this
** file. Please review the following information to ensure the GNU General
** Public License version 3.0 requirements will be met:
** http://www.gnu.org/copyleft/gpl.html.
**
****************************************************************************/

using System;
using System.Text.RegularExpressions;

namespace NdefLibrary.Ndef
{
    /// <summary>
    /// Record type using a custom URI scheme associated with the NearSpeak app
    /// to store voice messages on NFC tags.
    /// </summary>
    /// <remarks>
    /// Note that you are free to create NearSpeak NFC tags with your application.
    /// However, apps that register to the "nearspeak:" URL protocol and start upon
    /// tapping the tag must not be implemented without prior permission by Mopius.
    /// The protocol and name "NearSpeak" are proprietary to Mopius.
    /// 
    /// The NearSpeak app is a free application to record voice messages and store
    /// them as text (using speech recognition) on an NFC tag. Tapping the tag
    /// instantly launches the app again, in order to speak the text stored
    /// on the tag (using speech synthesis).
    /// 
    /// The custom URI scheme defined also contains the gender and language
    /// to be used for the speech synthesis, followed by the text to speak.
    /// 
    /// The cloud based tags are registered in a database on the Mopius server
    /// and always play in the user's language. Creating and querying the
    /// NearSpeak cloud database is only possible through the NearSpeak app.
    /// 
    /// More information: http://www.nearspeak.at/ 
    /// </remarks>
    public class NdefNearSpeakRecord : NdefUriRecord
    {
        private const string NearSpeakScheme = "nearspeak:";
        private const string GenderMaleString = "m";
        private const string GenderFemaleString = "f";
        private const string CloudProtocolIdentifier = "x";

        /// <summary>
        /// Genders available for the speech synthesizer.
        /// </summary>
        public enum NfcGender
        {
            Male = 0,
            Female
        }

        private NfcGender _gender;
        /// <summary>
        /// Gender to use for the speech synthesizer.
        /// </summary>
        public NfcGender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                UpdatePayload();
            }
        }

        /// <summary>
        /// Access the gender of the speech synthesizer as a string ("m" or "f")
        /// </summary>
        public string GenderAsString
        {
            get { return (Gender == NfcGender.Male ? GenderMaleString : GenderFemaleString); }
            set
            {
                if (value == GenderMaleString || value == GenderFemaleString)
                {
                    Gender = (value == GenderMaleString) ? NfcGender.Male : NfcGender.Female;
                }
            }
        }

        /// <summary>
        /// Convert the gender from the enum to the string used by the app.
        /// </summary>
        /// <param name="gender">Gender to convert to a string.</param>
        /// <returns>String representation of the gender.</returns>
        private string ConvertGenderToString(NfcGender gender)
        {
            return (gender == NfcGender.Male) ? GenderMaleString : GenderFemaleString;
        }

        private string _language;

        /// <summary>
        /// Language code to use for the speech synthesizer.
        /// Has to be 5 characters (e.g., en-US, de-DE, zh-CN, zh-HK, zh-TW)
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                UpdatePayload();
            }
        }

        private string _textToSpeak;

        /// <summary>
        /// The text to speak for the NearSpeak app.
        /// </summary>
        public string TextToSpeak
        {
            get { return _textToSpeak; }
            set
            {
                _textToSpeak = value;
                UpdatePayload();
            }
        }

        private string _cloudId;

        /// <summary>
        /// ID of the text stored on the NearSpeak online web-service.
        /// These tags can only be created by the official NearSpeak app.
        /// </summary>
        public string CloudId
        {
            get { return _cloudId; }
            set
            {
                _cloudId = value;
                UpdatePayload();
            }
        }

        public bool IsCloudBased()
        {
            return !String.IsNullOrEmpty(CloudId);
        }
        
        /// <summary>
        /// Create an empty NearSpeak record. You need to set the gender,
        /// language and text to speak to create a valid record.
        /// </summary>
        public NdefNearSpeakRecord()
        {
        }

        /// <summary>
        /// Create a telephone record based on another telephone record, or Smart Poster / URI
        /// record that have a Uri set that corresponds to the tel: URI scheme.
        /// </summary>
        /// <param name="other">Other record to copy the data from.</param>
        public NdefNearSpeakRecord(NdefRecord other)
            : base(other)
        {
            ParseUriToData(Uri);
        }

        /// <summary>
        /// Deletes any details currently stored in the NearSpeak record 
        /// and re-initializes them by parsing the contents of the provided URI.
        /// </summary>
        /// <remarks>The URI has to be formatted according to the nearspeak: URI scheme,
        /// and include the gender, language and text to speak *or* the gender and online ID.</remarks>
        private void ParseUriToData(string uri)
        {
            if (!uri.StartsWith(NearSpeakScheme))
                return;

            var textParameter = uri.Substring(NearSpeakScheme.Length);

            // Check for cloud-based ID
            // -----------------------------------------------------------------------------------------
            if (textParameter.Length < 3)    // 3... minimum length: xm1 (online, male, ID = 1)
                return;

            if (textParameter.Substring(0, 1) == CloudProtocolIdentifier)
            {
                var cloudGender = textParameter.Substring(1, 1);
                if (String.Compare(cloudGender, GenderFemaleString, StringComparison.Ordinal) == 0 || 
                    String.Compare(cloudGender, GenderMaleString, StringComparison.Ordinal) == 0)
                {
                    // Cloud-based message
                    _cloudId = textParameter.Substring(2, textParameter.Length - 2);
                    GenderAsString = cloudGender; // Will call UpdatePayload()
                    return;
                }
            }

            // Check for offline ID
            // -----------------------------------------------------------------------------------------
            // Extract product name and serial number from the payload
            var pattern = new Regex(NearSpeakScheme + @"(?<language>[a-zA-Z\-]{5})(?<gender>[mf])(?<textToSpeak>.*)");
            var match = pattern.Match(uri);
            // Assign extracted data to member variables
            if (match.Success)
            {
                _language = match.Groups["language"].Value;
                _textToSpeak = match.Groups["textToSpeak"].Value;
                GenderAsString = match.Groups["gender"].Value; // Will call UpdatePayload()
            }
        }

        /// <summary>
        /// Format the URI of the Uri base class.
        /// </summary>
        private void UpdatePayload()
        {
            if (IsCloudBased())
            {
                if (!String.IsNullOrEmpty(CloudId))
                    Uri = NearSpeakScheme + CloudProtocolIdentifier + GenderAsString + CloudId;
            }
            else
            {
                if (!String.IsNullOrEmpty(Language) && !String.IsNullOrEmpty(TextToSpeak))
                    Uri = NearSpeakScheme + Language + GenderAsString + TextToSpeak;
            }
        }

        /// <summary>
        /// Checks if the record sent via the parameter is indeed a NearSpeak record.
        /// Only checks the type and type name format, doesn't analyze if the
        /// payload is valid.
        /// </summary>
        /// <param name="record">Record to check.</param>
        /// <returns>True if the record has the correct type and type name format
        /// to be a NearSpeak record, false if it's a different record.</returns>
        public new static bool IsRecordType(NdefRecord record)
        {
            if (!NdefUriRecord.IsRecordType(record)) return false;
            var testRecord = new NdefUriRecord(record);
            if (testRecord.Uri == null || testRecord.Uri.Length < NearSpeakScheme.Length + 3)
                return false;
            return testRecord.Uri.StartsWith(NearSpeakScheme);
        }

        /// <summary>
        /// Checks if the contents of the record are valid; throws an exception if
        /// a problem is found, containing a textual description of the issue.
        /// </summary>
        /// <exception cref="NdefException">Thrown if no valid NDEF record can be
        /// created based on the record's current contents. The exception message 
        /// contains further details about the issue.</exception>
        /// <returns>True if the record contents are valid, or throws an exception
        /// if an issue is found.</returns>
        public override bool CheckIfValid()
        {
            // First check the basics
            if (!base.CheckIfValid()) return false;

            // Check specific content of this record
            if (IsCloudBased())
            {
                if (String.IsNullOrEmpty(CloudId))
                    throw new NdefException(NdefExceptionMessages.ExNearSpeakNoCloudId);
            }
            else
            {
                // Does the record contain text to speak
                if (string.IsNullOrEmpty(TextToSpeak))
                    throw new NdefException(NdefExceptionMessages.ExNearSpeakNoText);
                // Is the language defined?
                if (string.IsNullOrEmpty(Language))
                    throw new NdefException(NdefExceptionMessages.ExNearSpeakNoLanguage);
                // Is the language code 5 characters? (e.g., en-us is OK)
                if (Language.Length != 5)
                    throw new NdefException(NdefExceptionMessages.ExNearSpeakLanguageWrongLength);
            }
            return true;
        }
    }
}
