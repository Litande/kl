﻿//-----------------------------------------------------------------------------
// Filename: ISIPServerUserAgent.cs
//
// Description: The interface definition for SIP Server User Agents (UAS).

// Author(s):
// Aaron Clauson (aaron@sipsorcery.com)
//
// History:
// 30 Aug 2009	Aaron Clauson   Created, Hobart, Australia.
// rj2: added overloads for Answer/Reject/Redirect-methods with/out customHeader
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//-----------------------------------------------------------------------------

using SIPSorcery.core.SIP;
using SIPSorcery.core.SIPCDR;
using SIPSorcery.core.SIPTransactions;

namespace SIPSorcery.app.SIPUserAgents
{
    public delegate void SIPUASDelegate(ISIPServerUserAgent uas);

    /// <summary>
    /// Interface for classes implementing SIP server user agent functionality. The
    /// main function of a SIP client user agent is the ability to receive calls.
    /// </summary>
    public interface ISIPServerUserAgent
    {
        SIPCallDirection CallDirection { get; }
        SIPDialogue SIPDialogue { get; }
        UASInviteTransaction ClientTransaction { get; }
        ISIPAccount SIPAccount { get; set; }
        bool IsAuthenticated { get; set; }
        bool IsB2B { get; }
        bool IsInvite { get; }                      // Set to true for server user agents that are handling an INVITE request.
        SIPRequest CallRequest { get; }
        string CallDestination { get; }
        bool IsUASAnswered { get; }

        event SIPUASDelegate CallCancelled;
        event SIPUASDelegate NoRingTimeout;

        bool AuthenticateCall();
        void Progress(SIPResponseStatusCodesEnum progressStatus, string reasonPhrase, string[] customHeaders, string progressContentType, string progressBody);
        SIPDialogue Answer(string contentType, string body, SIPDialogueTransferModesEnum transferMode);
        SIPDialogue Answer(string contentType, string body, string toTag, SIPDialogueTransferModesEnum transferMode);
        SIPDialogue Answer(string contentType, string body, SIPDialogueTransferModesEnum transferMode, string[] customHeaders);
        SIPDialogue Answer(string contentType, string body, string toTag, SIPDialogueTransferModesEnum transferMode, string[] customHeaders);
        void Reject(SIPResponseStatusCodesEnum failureStatus, string reasonPhrase);
        void Reject(SIPResponseStatusCodesEnum failureStatus, string reasonPhrase, string[] customHeaders);
        void Redirect(SIPResponseStatusCodesEnum redirectCode, SIPURI redirectURI);
        void Redirect(SIPResponseStatusCodesEnum redirectCode, SIPURI redirectURI, string[] customHeaders);
        void NoCDR();
        void AnswerNonInvite(SIPResponseStatusCodesEnum answerStatus, string reasonPhrase, string[] customHeaders, string contentType, string body);
    }
}
